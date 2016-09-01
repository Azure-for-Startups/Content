var express = require('express');
var router = express.Router();

// getting user name ny Tags
function getNameByRegistrationTags(tags) {
	try {
		return JSON.parse(new Buffer(tags, 'base64').toString('utf8')).name;
	}
    catch (e) {}
	return tags;
}

/* GET registrations list listing. */
router.get('/', function(req, res, next) {
	// getting the registrations list
	notificationHubService.listRegistrations(null, function (error, response) {
		if (!error) {
			var regisrations = response.map(function (item) {
				return {
					name: getNameByRegistrationTags(item.Tags),
					tags: item.Tags,
					// identify registration type
					registrationType: item._.ContentRootElement.replace('RegistrationDescription', '')
				};
			});
			res.render('push', { title: 'Registrations', regisrations: regisrations });
		} else {
		    throw new Error(error);
		}
	});
});

// GET view for sending notification
router.get('/send/', function(req, res, next) {
	var tags = req.query.tags;
    var name = getNameByRegistrationTags(tags);
	res.render('send', { title: 'Send message', name: name, tags: tags, registrationType: req.query.registrationType, message: '' });
});

// POST send notification to use
router.post('/send/', function(req, res, next) {
	var message = req.body.text;
	var tags = req.query.tags;
	var name = getNameByRegistrationTags(tags);
	var registrationType = req.query.registrationType;
	var payload;
	
	// callback method after finished sending notification
    var onSendingCallback = function(error) {
        if (!error) {
            res.redirect('/push');
        } else {
            res.render('send', { title: 'Send message', name: name, tags: tags, registrationType: req.query.registrationType, message: message });
        }
    };
    switch (registrationType) {
		// sending notification to the Windows platform
		case 'Windows':
		    payload = '<toast><visual><binding template="ToastText01"><text id="1">' + message + '</text></binding></visual></toast>';
		    notificationHubService.wns.send(tags, payload, 'wns/toast', onSendingCallback);
			break;
			
		// sending notification to the Android platform
		case 'Gcm':
		    payload = {
		        data: {
		            msg: message
		        }
			};
			notificationHubService.gcm.send(tags, payload, onSendingCallback);
			break;
			
		// sending notification to the Apple platform
		case 'Apple':
			payload = {
				alert: message
			};
		    notificationHubService.apns.send(null, payload, onSendingCallback);
			break;
		default:
		    throw new Error('Unknowwn registration type');
    }
});

module.exports = router;
