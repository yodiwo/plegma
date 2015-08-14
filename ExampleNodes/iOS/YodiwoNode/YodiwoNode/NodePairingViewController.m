//
//  NodePairingViewController.m
//  YodiwoNode
//
//  Created by r00tb00t on 7/26/15.
//  Copyright (c) 2015 yodiwo. All rights reserved.
//

#import "NodePairingViewController.h"
#import "NodePairingService.h"
#import "SettingsVault.h"
#import "TWMessageBarManager.h"


@interface NodePairingViewController ()

// Privates
@property (strong, nonatomic) NodePairingService *nodePairingService;
@property (strong, nonatomic) UIWebView *pairingLoginWebView;
@property (strong, nonatomic) SettingsVault *sharedSettingsVault;

// Outlets
@property (weak, nonatomic) IBOutlet UIButton *startNodeButton;

@end


@implementation NodePairingViewController

///***** Override synthesized getters, lazy instantiate

- (NodePairingService *)nodePairingService {
    if (!_nodePairingService) {
        _nodePairingService = [[NodePairingService alloc] init];
    }

    return _nodePairingService;
}

- (UIWebView *)pairingLoginWebView {
    if(!_pairingLoginWebView) {
        _pairingLoginWebView = [[UIWebView alloc]
                                initWithFrame:[[UIScreen mainScreen]
                                               applicationFrame]];
        _pairingLoginWebView.scalesPageToFit = YES;
    }

    return _pairingLoginWebView;
}

-(SettingsVault *)sharedSettingsVault {
    if (!_sharedSettingsVault) {
        _sharedSettingsVault = [SettingsVault sharedSettingsVault];
    }

    return _sharedSettingsVault;
}
//******************************************************************************





///***** Functionality

// Entry points
- (IBAction)startNodeButtonPressed:(UIButton *)sender {
    dispatch_async(dispatch_get_main_queue(), ^{
        [sender setEnabled:NO];
    });

    if(![self.sharedSettingsVault isNodePaired]) {
        [self.nodePairingService initiatePairingWithCompletionHandler:^(NSString *pairingWebLoginUrl) {
            NSLog(@"Starting web view to URL -> %@", pairingWebLoginUrl);

            dispatch_async(dispatch_get_main_queue(), ^{
                NSURL *url = [NSURL URLWithString:pairingWebLoginUrl];
                NSURLRequest *urlRequest = [NSURLRequest requestWithURL:url];
                [self.pairingLoginWebView loadRequest:urlRequest];
            });
        }];
    }
    else {
        // Already paired, move to main application content
        dispatch_async(dispatch_get_main_queue(), ^{
            [self performSegueWithIdentifier:@"nodePairedMoveToMainAppContent" sender:self];
        });
    }
}

- (IBAction)unpairNodeButtonPressed:(UIButton *)sender {
    [[SettingsVault sharedSettingsVault] setIsNodePaired:NO];

    [self alertUserWithTitle:@"Yodiwo Node info"
              showingMessage:@"Node unpaired. Start node to initiate repairing."
                 actionTitle:@"OK"];
}

// Delegates

// UIWebView
- (void)webViewDidFinishLoad:(UIWebView *)webView {
    if([webView.request.URL.absoluteString hasSuffix:@"/pairing/success"]) {
        NSLog(@"Pairing phase 1 completed succesfully");

        [self.pairingLoginWebView removeFromSuperview];

        [self.nodePairingService finalizePairingWithCompletionHandler:^(BOOL result) {
            dispatch_async(dispatch_get_main_queue(), ^{
                [[TWMessageBarManager sharedInstance] showMessageWithTitle:@"Node info:"
                                                               description:(result == YES) ?
                                                                            @"Pairing successful" :
                                                                            @"Pairing failed"
                                                                      type:TWMessageBarMessageTypeInfo
                                                                  duration:3.0];

                // Paired succesfully, move to main application content
                dispatch_async(dispatch_get_main_queue(), ^{
                    [self performSegueWithIdentifier:@"nodePairedMoveToMainAppContent" sender:self];
                });
            });
        }];
    }
    else {
        [self.view addSubview:self.pairingLoginWebView];
    }
}

- (void)webViewDidStartLoad:(UIWebView *)webView {

}

- (void)webView:(UIWebView *)webView didFailLoadWithError:(NSError *)error {
    NSLog(@"Failed to get tokens");
}

- (BOOL)webView:(UIWebView *)webView shouldStartLoadWithRequest:(NSURLRequest *)request navigationType:(UIWebViewNavigationType)navigationType {
    return YES;
}
//******************************************************************************





///***** View related

- (void)viewDidLoad {
    [super viewDidLoad];

    [self.pairingLoginWebView setDelegate:self];
}

- (void)viewWillAppear:(BOOL)animated {

}

- (void)viewWillDisappear:(BOOL)animated {

}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

 #pragma mark - Navigation

 - (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
     // Get the new view controller using [segue destinationViewController].
     // Pass the selected object to the new view controller.
 }
//******************************************************************************





///***** Helpers

-(void) alertUserWithTitle:(NSString *)title
            showingMessage:(NSString *)message
               actionTitle:(NSString *)action {

    UIAlertController *sharingLevelInfoAlert =
            [UIAlertController alertControllerWithTitle:title
                                                message:message
                                         preferredStyle:UIAlertControllerStyleAlert];

    [sharingLevelInfoAlert addAction:[UIAlertAction actionWithTitle:action
                                                              style:UIAlertActionStyleDefault
                                                            handler:^(UIAlertAction * action) {}]];

    [self presentViewController:sharingLevelInfoAlert animated:YES completion:nil];
}
//******************************************************************************

@end
