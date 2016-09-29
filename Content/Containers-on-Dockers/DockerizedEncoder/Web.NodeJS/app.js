var express = require('express');
var path = require('path');
var favicon = require('serve-favicon');
var logger = require('morgan');
var cookieParser = require('cookie-parser');
var bodyParser = require('body-parser');
var routes = require('./routes/index');
var users = require('./routes/users');

var uuid = require('node-uuid');
var file = require('fs');
var Client = require('ssh2').Client;
var azure = require('azure-storage');

var app = express();

config = require('./config.json');
util = require('util');

storageBlobService = azure.createBlobService(config.storage.accountName, config.storage.secret);

startDocker = function(){
	throw new Error();
};

var conn = new Client();
conn.on('ready', function() {
	console.log('Client :: ready');
	startDocker = function(fileUrl, processId) {
		var command = util.format(config.docker.executeCommadTemplate, config.docker.port, config.docker.imageName, fileUrl, config.storage.accountName, config.storage.secret, config.storage.containerName, processId);
		console.log(command);
		conn.exec(command, function(err, stream) {
			if (err) throw err;
			stream.on('data', function(data) {
				console.log('STDOUT: ' + data);
			}).stderr.on('data', function(data) {
				console.log('STDERR: ' + data);
			});
		});
	}
}).connect({
	host: config.docker.ssh.host,
	port: config.docker.ssh.port,
	username: config.docker.ssh.username,
	privateKey: file.readFileSync(config.docker.ssh.pathToPrivateKey)
});

guidGenerator = uuid;
// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'jade');

// uncomment after placing your favicon in /public
//app.use(favicon(__dirname + '/public/favicon.ico'));
app.use(logger('dev'));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(require('stylus').middleware(path.join(__dirname, 'public')));
app.use(express.static(path.join(__dirname, 'public')));

app.use('/', routes);
app.use('/users', users);

// catch 404 and forward to error handler
app.use(function (req, res, next) {
    var err = new Error('Not Found');
    err.status = 404;
    next(err);
});

// error handlers

// development error handler
// will print stacktrace
if (app.get('env') === 'development') {
    app.use(function (err, req, res, next) {
        res.status(err.status || 500);
        res.render('error', {
            message: err.message,
            error: err
        });
    });
}

// production error handler
// no stacktraces leaked to user
app.use(function (err, req, res, next) {
    res.status(err.status || 500);
    res.render('error', {
        message: err.message,
        error: {}
    });
});


module.exports = app;
