var maxRetries = 3;
var blockLength = 1048576;
var numberOfBlocks = 1;
var currentChunk = 1;
var retryAfterSeconds = 3;

$(document).ready(function() {
    $(document).on("click", "#button_upload", beginUpload);
    $("#progressBar").progressbar(0);
});

var beginUpload = function() {
    var fileControl = document.getElementById("input_video_file");
    if (fileControl.files.length > 0) {
        displayStatusMessage("Sending Metadata");
        uploadMetaData(fileControl.files[0]);
    } else {
        alert("Please select the file first");
    }
};
var uploadMetaData = function(file) {
    var size = file.size;
    numberOfBlocks = Math.ceil(file.size / blockLength);
    var name = file.name;
    currentChunk = 1;

    $.ajax({
            type: "POST",
            async: false,
            url: "/VideoElement/SetMetadata?blocksCount=" + numberOfBlocks + "&fileName=" + name + "&fileSize=" + size,
        })
        .done(function(state) {
            if (state === true) {
                displayStatusMessage("Starting Upload");
                sendFile(file, blockLength);
            }
        })
        .fail(function() {
            displayStatusMessage("Failed to send MetaData");
        });

};
var sendFile = function(file, chunkSize) {
    var start = 0,
        end = Math.min(chunkSize, file.size),
        retryCount = 0,
        sendNextChunk,
        fileChunk;

    sendNextChunk = function() {
        fileChunk = new FormData();

        if (file.slice) {
            fileChunk.append("Slice", file.slice(start, end));
        } else if (file.webkitSlice) {
            fileChunk.append("Slice", file.webkitSlice(start, end));
        } else if (file.mozSlice) {
            fileChunk.append("Slice", file.mozSlice(start, end));
        } else {
            displayStatusMessage(operationType.UNSUPPORTED_BROWSER);
            return;
        }

        jqxhr = $.ajax({
                async: true,
                url: ("/VideoElement/UploadChunk?id=" + currentChunk),
                data: fileChunk,
                cache: false,
                contentType: false,
                processData: false,
                type: "POST"
            })
            .fail(function(request, error) {
                if (error !== "abort" && retryCount < maxRetries) {
                    ++retryCount;
                    setTimeout(sendNextChunk, retryAfterSeconds * 1000);
                }

                if (error === "abort") {
                    displayStatusMessage("Aborted");
                } else {
                    if (retryCount === maxRetries) {
                        displayStatusMessage("Upload timed out.");
                        resetControls();
                        uploader = null;
                    } else {
                        displayStatusMessage("Resuming Upload");
                    }
                }

                return;
            })
            .done(function(result) {
                if (result.error || result.isLastBlock) {
                    if (result.isLastBlock) {
                        $("#AssetId").val(result.assetId);
                        $("#button_create").prop("disabled", false);
                    }
                    displayStatusMessage(result.message);
                    return;
                }
                ++currentChunk;
                start = (currentChunk - 1) * blockLength;
                end = Math.min(currentChunk * blockLength, file.size);
                retryCount = 0;
                updateProgress();
                if (currentChunk <= numberOfBlocks) {
                    sendNextChunk();
                }
            });
    };
    sendNextChunk();
};
var displayStatusMessage = function(message) {
    $("#statusMessage").text(message);
};
var updateProgress = function() {
    var progress = currentChunk / numberOfBlocks * 100;
    if (progress <= 100) {
        $("#progressBar").progressbar("option", "value", parseInt(progress));

        var message = "Uploaded " + progress + "%";
        if (progress === 100) {
            message += ". Creating the Asset...";
        }

        displayStatusMessage(message);
    }

};