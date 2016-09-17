var express = require('express');
var router = express.Router();

function getIndexPage(req, res) {
	// get user Id
	var userId = req.cookies.id;

	if (!userId) {
		// if user doesn't have id then generate Id
		userId = guidGenerator.v4().replace(/-/g, '');
	    res.cookie("id", userId);
	}
	storageBlobService.listBlobsSegmentedWithPrefix(config.storage.containerName, userId, null, function(error, result, response) {
		if (!error) {
			//getting all tasks
			var allTasks = result.entries.map(function (blob) {
				// build uri to download
		        var task = {blobSource: util.format(config.storage.filePathTemplate, config.storage.accountName, config.storage.containerName, blob.name)};
				var firstPos = blob.name.indexOf('_');
				var secondPos = blob.name.indexOf('_', firstPos + 1);
				// getting task status
				task.status = blob.name.substr(firstPos + 1, secondPos - firstPos - 1);
			    return task;
			});
			res.render('index', { title: 'Dockerized Encoder on Azure' , processes: allTasks});
		} else {
			// if error then return empty page
		    console.log(error);
			res.render('index', { title: 'Dockerized Encoder on Azure' , processes: [] });
		}
	});
}

/* GET home page. */
router.get('/', function (req, res, next) {
	getIndexPage(req, res);
});

router.post('/', function (req, res, next) {
	// getting process Id
	var processId = req.cookies.id + guidGenerator.v4().replace(/-/g, '');
	// encode url
	var fileUrl = encodeURI(req.body.fileUrl);
	// execute sending command to docker via ssh
	startDocker(fileUrl, processId);
	getIndexPage(req, res);
});

module.exports = router;
