//
//  AppDelegate.h
//  PushMeNow.iOS
//

#import <UIKit/UIKit.h>
#import "UserInfo.h"

@interface AppDelegate : UIResponder <UIApplicationDelegate>

@property (strong, nonatomic) UIWindow *window;

+(instancetype) sharedAppDelegate;
-(void) registerPushNotificationForUser:(UserInfo*) userInfo;

@end

