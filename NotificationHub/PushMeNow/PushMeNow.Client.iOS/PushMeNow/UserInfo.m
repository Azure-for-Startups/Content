//
//  UserInfo.m
//  PushMeNow
//

#import "UserInfo.h"

NSString* createUUID()
{
    CFUUIDRef uuid = CFUUIDCreate(NULL);
    CFStringRef str = CFUUIDCreateString(NULL, uuid);
    CFRelease(uuid);
    return (__bridge NSString*)str;
}


@interface UserInfo ()

@property (retain) NSString * salt;

@end

@implementation UserInfo

-(void)setName:(NSString *)name {
    _name = name;
    int saltLength = 7;
    do {
        ++saltLength;
        self.salt = [createUUID() substringToIndex:saltLength];
    }
    while ([self.asTag containsString:@"="]);
}

-(NSString*) asTag {
    NSDictionary * obj = @{@"name": self.name ?: @"",
                           @"salt": self.salt ?: @""
                           };
    return [[NSJSONSerialization dataWithJSONObject:obj options:nil error:nil]
            base64EncodedStringWithOptions:nil];
}

@end
