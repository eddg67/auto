// WARNING
// This file has been generated automatically by Xamarin Studio to
// mirror C# types. Changes in this file made by drag-connecting
// from the UI designer will be synchronized back to C#, but
// more complex manual changes may not transfer correctly.


#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>


@interface LoginScreenController : UIViewController {
}
- (IBAction)validateInputs:(UIButton *)sender forEvent:(UIEvent *)event;
- (IBAction)validateInput:(id *)sender forEvent:(UIEvent *)event;
@property (retain, nonatomic) IBOutlet UITextField *txtEmail;
@property (retain, nonatomic) IBOutlet UIImageView *txtPassword;
@property (retain, nonatomic) IBOutlet UIButton *btnSignUp;
@property (retain, nonatomic) IBOutlet UIButton *btnForgotPassword;

@property (retain, nonatomic) IBOutlet UIButton *btnSubmitLogin;
@end
