Using your own MsgData:

1. Implement your own protos repository following below structure:

```shell
├───ciot
│   └───proto
│       └───v2
|           └───msg_data.proto
└───custom
    └───proto
        └───v1
            └───custom_data.proto
```

msg_data.proto:
```protobuf
syntax = "proto3";

package Ciot;

import "ciot/proto/v2/ciot.proto";
import "ciot/proto/v2/http_client.proto";
import "ciot/proto/v2/http_server.proto";
import "ciot/proto/v2/iface.proto";
import "ciot/proto/v2/mqtt_client.proto";
import "ciot/proto/v2/uart.proto";
import "ciot/proto/v2/sys.proto";
import "ciot/proto/v2/ntp.proto";
import "ciot/proto/v2/ble_adv.proto";
import "ciot/proto/v2/ble_scn.proto";
import "ciot/proto/v2/ble.proto";
import "ciot/proto/v2/dfu.proto";
import "ciot/proto/v2/gpio.proto";
import "ciot/proto/v2/ota.proto";
import "ciot/proto/v2/storage.proto";
import "ciot/proto/v2/tcp.proto";
import "ciot/proto/v2/wifi.proto";
import "ciot/proto/v2/logger.proto";
import "ciot/proto/v2/usb.proto";
import "ciot/proto/v2/mbus_client.proto";
import "ciot/proto/v2/mbus_server.proto";
import "custom/proto/v1/custom.proto";         // <<---- Your main custom proto file
import "google/protobuf/any.proto";

option csharp_namespace = "Ciot.Protos.V2";

message MsgData {
  oneof type {
    Common common = 1;                          // Common data.
    GetData get_data = 2;                       // Get data request.
    Ciot.Data ciot = 3;                         // CioT data.
    SysData sys = 4;                            // System data.
    HttpClientData http_client = 5;             // HTTP Client data.
    HttpServerData http_server = 6;             // HTTP Server data.
    MqttClientData mqtt_client = 7;             // MQTT Client data.
    UartData uart = 8;                          // UART data.
    NtpData ntp = 9;                            // NTP data.
    BleAdvData ble_adv = 10;                    // BLE adv data.
    BleScnData ble_scn = 11;                    // BLE scanner data.
    BleData ble = 12;                           // BLE data.
    DfuData dfu = 13;                           // DFU data.
    GpioData gpio = 14;                         // GPIO data.
    OtaData ota = 15;                           // OTA data.
    StorageData storage = 16;                   // Storage data.
    TcpData eth = 17;                           // Ethernet data.
    WifiData wifi = 18;                         // WiFi data.
    LogData log = 19;                           // Log data.
    UsbData usb = 20;                           // USB data.
    MbusClientData mbus_client = 21;            // Modbus client data.
    MbusServerData mbus_server = 22;            // Modbus server data.
    CustomPackage.CustomData custom_data = 100; // <<---- Your main custom proto message
  }
}
```

**Hint:** Only `common`, `get_data`, `ciot` and `sys` fields are mandatory on MsgData. You can remove any other field that you don't use. You can use any name for your proto files and messages instead `custom.proto`, `CustomPackage`, etc.

1. Add ciot_cs as submodule:

```shell
git submodule add https://github.com/ciot-platform/ciot_cs.git submodules/Ciot
```

2. Update Ciot submodule:

```shell
git submodule update --init --recursive
```

3. Add your own protos submodule:

```
git submodule add {your-proto-repository} submodules/Protos
```

4. Run Ciot GRPC server:

```shell
dotnet run --project submodules\Ciot\Ciot.Grpc\Ciot.Grpc.csproj
```
