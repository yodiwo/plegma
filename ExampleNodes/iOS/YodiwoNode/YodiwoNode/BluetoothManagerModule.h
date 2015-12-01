//
//  BluetoothManager.h
//  YodiwoHub
//
//  Created by r00tb00t on 10/12/15.
//  Copyright © 2015 yodiwo. All rights reserved.
//

@import Foundation;
@import CoreBluetooth;

@interface BluetoothManagerModule : NSObject<CBCentralManagerDelegate, CBPeripheralDelegate>

- (void)start;

- (void)stop;

@end
