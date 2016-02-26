//
//  BluetoothManager.m
//  YodiwoHub
//
//  Created by r00tb00t on 10/12/15.
//  Copyright © 2015 yodiwo. All rights reserved.
//

#import "BluetoothManagerModule.h"

#import "NodeThingsRegistry.h"
#import "NodeController.h"
#import "SettingsVault.h"


@interface BluetoothManagerModule ()

@property (strong, nonatomic) CBCentralManager *bluetoothCentralManager;

@end

@implementation BluetoothManagerModule

///***** Override synthesized getters, lazy instantiate

-(CBCentralManager *)bluetoothCentralManager {
    if(_bluetoothCentralManager == nil) {

        dispatch_queue_t newQueue = dispatch_queue_create("BluetoothManagerModuleQueue", DISPATCH_QUEUE_SERIAL);
        _bluetoothCentralManager = [[CBCentralManager alloc] initWithDelegate:self queue:newQueue];
    }

    return _bluetoothCentralManager;
}

//******************************************************************************



///***** Public api

- (void)start
{
    // Dummy
    if (self.bluetoothCentralManager == nil) { return; }
}

- (void)stop
{
    [self.bluetoothCentralManager stopScan];
}

//******************************************************************************



///***** Delegates

#pragma mark - CBCentralManagerDelegate

- (void)centralManagerDidUpdateState:(CBCentralManager *)central
{
    CBCentralManagerState state = [central state];
    NSString *statusStr = @"";

    if (state == CBCentralManagerStatePoweredOff) {
        NSLog(@"CoreBluetooth BLE hardware is powered off");
        statusStr = @"Powered Off";
    }
    else if (state == CBCentralManagerStatePoweredOn) {
        NSLog(@"CoreBluetooth BLE hardware is powered on and ready");
        statusStr = @"Powered On and Ready";

        [self.bluetoothCentralManager scanForPeripheralsWithServices:nil options:nil];
    }
    else if (state == CBCentralManagerStateUnauthorized) {
        NSLog(@"CoreBluetooth BLE state is unauthorized");
        statusStr = @"Unauthorized";
    }
    else if (state == CBCentralManagerStateUnknown) {
        NSLog(@"CoreBluetooth BLE state is unknown");
        statusStr = @"Unknown";
    }
    else if (state == CBCentralManagerStateUnsupported) {
        NSLog(@"CoreBluetooth BLE hardware is unsupported on this platform");
        statusStr = @"Not supported";
    }

    // Notify cloud service
    [[NodeController sharedNodeController] sendSinglePortEventMsgFromThing:ThingNameBluetoothStatus
                                                             fromPortIndex:0
                                                                 withState:statusStr];
}

- (void)centralManager:(CBCentralManager *)central didConnectPeripheral:(CBPeripheral *)peripheral
{
    [peripheral setDelegate:self];
    [peripheral discoverServices:nil];

    NSString *state = peripheral.state == CBPeripheralStateConnected ? @"CONNECTED" : @"NOT CONNECTED";
    NSLog(@"Peripheral %@ state: %@", peripheral.name, state);

    if (peripheral.state == CBPeripheralStateConnected) {
        // Notify cloud service
        [[NodeController sharedNodeController] sendSinglePortEventMsgFromThing:ThingNameBluetoothStatus
                                                                 fromPortIndex:3
                                                                     withState:peripheral.name];
    }
}

- (void)centralManager:(CBCentralManager *)central
 didDiscoverPeripheral:(CBPeripheral *)peripheral
     advertisementData:(NSDictionary *)advertisementData
                  RSSI:(NSNumber *)RSSI
{
    NSString *localName = [advertisementData objectForKey:CBAdvertisementDataLocalNameKey];
    if (localName != nil || [localName length] > 0 || peripheral.identifier.UUIDString != nil) {

        NSString *toSend;
        if (localName != nil && [localName length] > 0 ) {
            toSend = localName;
        }
        else {
            localName = @"Not advertised";
            toSend = [NSString stringWithString:peripheral.identifier.UUIDString];
        }

        NSLog(@"Found BLE peripheral: Name: %@, UUID: %@, RSSI = %@",
              localName, peripheral.identifier.UUIDString, RSSI);

        // RSSI might translate to distance indication
        // TODO:


        // Notify cloud service
        [[NodeController sharedNodeController] sendSinglePortEventMsgFromThing:ThingNameBluetoothStatus
                                                                 fromPortIndex:1
                                                                     withState:toSend];

        [[NodeController sharedNodeController] sendSinglePortEventMsgFromThing:ThingNameBluetoothStatus
                                                                 fromPortIndex:2
                                                                     withState:[RSSI stringValue]];
    }
}


#pragma mark - CBPeripheralDelegate

// CBPeripheralDelegate - Invoked when you discover the peripheral's available services.
- (void)peripheral:(CBPeripheral *)peripheral didDiscoverServices:(NSError *)error
{

}

// Invoked when you discover the characteristics of a specified service.
- (void)peripheral:(CBPeripheral *)peripheral didDiscoverCharacteristicsForService:(CBService *)service error:(NSError *)error
{

}

// Invoked when you retrieve a specified characteristic's value, or when the peripheral device notifies your app that the characteristic's value has changed.
- (void)peripheral:(CBPeripheral *)peripheral didUpdateValueForCharacteristic:(CBCharacteristic *)characteristic error:(NSError *)error
{

}

//******************************************************************************

@end