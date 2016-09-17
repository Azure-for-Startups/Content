// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include <stdlib.h>
#include <stdio.h>
//#include <stdint.h>

#include "IoTsample.h"
#include "Config\Config.h"

//#include <time.h>
//#include <stdio.h>
//#include <stdlib.h>
//#include <sys/types.h>
//#include <sys/timeb.h>
//#include <string.h>

int main(int argc, char *argv[])
{
    int result = 0;
    const char * configFilePath = argc > 1 ? argv[1] : "./IoTsample.conf";
    CONFIG_HANDLE conf = InitConfig(configFilePath);
    if (NULL == conf)
    {
        printf("Cannot open configuration file: \"%s\".", configFilePath);
        getchar();
        result = 1;
    }
    else
    {
        const char * deviceName = GetStringParameterValue(conf, "DeviceName", NULL);
        const char * connStr = GetStringParameterValue(conf, "ConnectionString", NULL);
        if (NULL != deviceName && NULL != connStr)
        {
            IoTsample_run(connStr, deviceName
                , GetDoubleParameterValue(conf, "Longitude", 39.1)
                , GetDoubleParameterValue(conf, "Latitude", 47.2)
                , GetDoubleParameterValue(conf, "PressureChangeSpeed", 15.25)
                , GetDoubleParameterValue(conf, "StartPressure", 100.18)
                , GetDoubleParameterValue(conf, "StopPressure", 1010.31)
                , (int)(GetDoubleParameterValue(conf, "EventPeriodMinutes", 5.0) * 60.0 * 1000.0)
                , GetBooleanParameterValue(conf, "Infinity", false)
                );
        }
        else
        {
            printf("There are DeviceName or ConnectionString parameter(s) in \"%s\" configuration file.", configFilePath);
            getchar();
            result = 2;
        }
    }
    ReleaseConfig(conf);
    return result;
}
