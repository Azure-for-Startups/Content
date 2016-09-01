// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#ifndef IOTSAMPLEHTTP_H
#define IOTSAMPLEHTTP_H

#include <stdbool.h>

#ifdef __cplusplus
extern "C" {
#endif

    void IoTsample_run(const char * connectionString
        , const char * deviceName
        , double longitude
        , double latitude
        , double pressureChangeSpeed
        , double startPressure
        , double stopPressure
        , int eventPeriodMilliseconds
        , bool isInfinity);

#ifdef __cplusplus
}
#endif

#endif /* SIMPLESAMPLEHTTP_H */
