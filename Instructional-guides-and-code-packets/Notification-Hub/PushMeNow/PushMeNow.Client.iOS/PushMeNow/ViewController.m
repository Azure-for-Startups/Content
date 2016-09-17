//
//  ViewController.m
//  PushMeNow.iOS
//

#import "ViewController.h"
#import "UserInfo.h"
#import "AppDelegate.h"

@interface ViewController ()

@property (strong, nonatomic) IBOutlet UITextField *userNameTextEdit;
@end

@implementation ViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view, typically from a nib.
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

- (IBAction)doRegister:(id)sender {
    UserInfo* userInfo = [UserInfo new];
    userInfo.name = self.userNameTextEdit.text;
    [[AppDelegate sharedAppDelegate] registerPushNotificationForUser:userInfo];
}
@end
