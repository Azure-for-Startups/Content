// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include <stdlib.h>

#include <stdio.h>
#include <stdint.h>

/* This sample uses the _LL APIs of iothub_client for example purposes.
That does not mean that HTTP only works with the _LL APIs.
Simply changing the using the convenience layer (functions not having _LL)
and removing calls to _DoWork will yield the same results. */

#include "serializer.h"
#include "iothub_client_ll.h"
#include "iothubtransporthttp.h"
#include "threadapi.h"

#ifdef MBED_BUILD_TIMESTAMP
#include "certs.h"
#endif // MBED_BUILD_TIMESTAMP

//HostName=IoThubtest1.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=zQKDEfUP2Nhs4RW0xkG75cea1q2QqO0Gg6XCfFJ7miM=

//static const char* connectionString = "HostName=IoThubtest1.azure-devices.net;DeviceId=TheFirstDevice;SharedAccessKey=xESdmy41C3BCdXrVnO5BqlrCGzqh4YcujygIOnsKLSc=";

//static const char* connectionString = "HostName=IoThubtest1.azure-devices.net;DeviceId=TheFirstDevice;SharedAccessKey=xESdmy41C3BCdXrVnO5BqlrCGzqh4YcujygIOnsKLSc=";

// Define the Model
BEGIN_NAMESPACE(WeatherStation);

DECLARE_MODEL(ContosoAnemometer,
WITH_DATA(ascii_char_ptr, DeviceName),
WITH_DATA(double, Longitude),
WITH_DATA(double, Latitude),
WITH_DATA(double, Pressure),
WITH_DATA(double, WindSpeed),
WITH_ACTION(TurnFanOn),
WITH_ACTION(TurnFanOff),
WITH_ACTION(SetAirResistance, int, Position)
);

END_NAMESPACE(WeatherStation);

DEFINE_ENUM_STRINGS(IOTHUB_CLIENT_CONFIRMATION_RESULT, IOTHUB_CLIENT_CONFIRMATION_RESULT_VALUES)

EXECUTE_COMMAND_RESULT TurnFanOn(ContosoAnemometer* device)
{
    (void)device;
    (void)printf("Turning fan on.\r\n");
    return EXECUTE_COMMAND_SUCCESS;
}

EXECUTE_COMMAND_RESULT TurnFanOff(ContosoAnemometer* device)
{
    (void)device;
    (void)printf("Turning fan off.\r\n");
    return EXECUTE_COMMAND_SUCCESS;
}

EXECUTE_COMMAND_RESULT SetAirResistance(ContosoAnemometer* device, int Position)
{
    (void)device;
    (void)printf("Setting Air Resistance Position to %d.\r\n", Position);
    return EXECUTE_COMMAND_SUCCESS;
}

void sendCallback(IOTHUB_CLIENT_CONFIRMATION_RESULT result, void* userContextCallback)
{
    int messageTrackingId = (intptr_t)userContextCallback;

    //(void)printf("Message Id: %d Received.\r\n", messageTrackingId);

    //(void)printf("Result Call Back Called! Result is: %s \r\n", ENUM_TO_STRING(IOTHUB_CLIENT_CONFIRMATION_RESULT, result));
}

static void sendMessage(IOTHUB_CLIENT_LL_HANDLE iotHubClientHandle, const unsigned char* buffer, size_t size)
{
    static unsigned int messageTrackingId;
    IOTHUB_MESSAGE_HANDLE messageHandle = IoTHubMessage_CreateFromByteArray(buffer, size);
    if (messageHandle == NULL)
    {
        printf("unable to create a new IoTHubMessage\r\n");
    }
    else
    {
        if (IoTHubClient_LL_SendEventAsync(iotHubClientHandle, messageHandle, sendCallback, (void*)(uintptr_t)messageTrackingId) != IOTHUB_CLIENT_OK)
        {
            printf("failed to hand over the message to IoTHubClient");
        }
        else
        {
            printf("IoTHubClient accepted the message for delivery\r\n");
        }
        IoTHubMessage_Destroy(messageHandle);
    }
    free((void*)buffer);
    messageTrackingId++;
}

/*this function "links" IoTHub to the serialization library*/
static IOTHUBMESSAGE_DISPOSITION_RESULT IoTHubMessage(IOTHUB_MESSAGE_HANDLE message, void* userContextCallback)
{
    IOTHUBMESSAGE_DISPOSITION_RESULT result;
    const unsigned char* buffer;
    size_t size;
    if (IoTHubMessage_GetByteArray(message, &buffer, &size) != IOTHUB_MESSAGE_OK)
    {
        printf("unable to IoTHubMessage_GetByteArray\r\n");
        result = EXECUTE_COMMAND_ERROR;
    }
    else
    {
        /*buffer is not zero terminated*/
        char* temp = malloc(size + 1);
        if (temp == NULL)
        {
            printf("failed to malloc\r\n");
            result = EXECUTE_COMMAND_ERROR;
        }
        else
        {
            memcpy(temp, buffer, size);
            temp[size] = '\0';
            EXECUTE_COMMAND_RESULT executeCommandResult = EXECUTE_COMMAND(userContextCallback, temp);
            result =
                (executeCommandResult == EXECUTE_COMMAND_ERROR) ? IOTHUBMESSAGE_ABANDONED :
                (executeCommandResult == EXECUTE_COMMAND_SUCCESS) ? IOTHUBMESSAGE_ACCEPTED :
                IOTHUBMESSAGE_REJECTED;
            free(temp);
        }
    }
    return result;
}

void IoTsample_run(const char * connectionString
    , const char * deviceName
    , double longitude
    , double latitude
    , double pressureChangeSpeed
    , double startPressure
    , double stopPressure
    , int eventPeriodMilliseconds
    , bool isInfinity)
{
    printf("IoT sample is started with parameters:\r\n"
        "ConnectionString: \r\n\"%s\"\r\n\r\n"
        "DeviceName: \t\t\"%s\"\r\n"
        "StartPressure: \t\t%.3f\r\n"
        "StopPressure: \t\t%.3f\r\n"
        "PressureChangeSpeed: \t%.3f\r\n"
        "Longitude: \t\t%.3f\r\n"
        "Latitude: \t\t%.3f\r\n"
        "Infinity: \t\t%s\r\n"
        , connectionString
        , deviceName
        , startPressure
        , stopPressure
        , pressureChangeSpeed
        , longitude
        , latitude
        , isInfinity ? "Yes" : "No"
        );
    if (serializer_init(NULL) != SERIALIZER_OK)
    {
        (void)printf("Failed on serializer_init\r\n");
    }
    else
    {
        IOTHUB_CLIENT_LL_HANDLE iotHubClientHandle = IoTHubClient_LL_CreateFromConnectionString(connectionString, HTTP_Protocol);
        srand((unsigned int)time(NULL));
        double avgWindSpeed = 10.0;

        if (iotHubClientHandle == NULL)
        {
            (void)printf("Failed on IoTHubClient_LL_Create\r\n");
        }
        else
        {
            unsigned int minimumPollingTime = 9; /*because it can poll "after 9 seconds" polls will happen effectively at ~10 seconds*/
            if (IoTHubClient_LL_SetOption(iotHubClientHandle, "MinimumPollingTime", &minimumPollingTime) != IOTHUB_CLIENT_OK)
            {
                printf("failure to set option \"MinimumPollingTime\"\r\n");
            }

            ContosoAnemometer* myWeather = CREATE_MODEL_INSTANCE(WeatherStation, ContosoAnemometer);
            if (myWeather == NULL)
            {
                (void)printf("Failed on CREATE_MODEL_INSTANCE\r\n");
            }
            else
            {
                if (IoTHubClient_LL_SetMessageCallback(iotHubClientHandle, IoTHubMessage, myWeather) != IOTHUB_CLIENT_OK)
                {
                    printf("unable to IoTHubClient_SetMessageCallback\r\n");
                }
                else
                {
                    //double mlsecsPerHour = 60 * 60 * 1000;
                    //double incPressure = pressureChangeSpeed / (mlsecsPerHour / eventPeriodMilliseconds);

                    double incPressure = pressureChangeSpeed / 3600.0;
                    bool regressPressure = startPressure > stopPressure;
                    if (regressPressure)
                    {
                        incPressure = -incPressure;
                    }

                    myWeather->DeviceName = (ascii_char_ptr)deviceName;

                    myWeather->Pressure = startPressure;
                    myWeather->Latitude = latitude;
                    myWeather->Longitude = longitude;
                    int seqNumber = 0;

                    double prevTime = (double)time(NULL);
                    while(1)
                    {
                        if ((regressPressure && myWeather->Pressure <= stopPressure) || (!regressPressure && myWeather->Pressure >= stopPressure))
                        {
                            if (isInfinity)
                            {
                                regressPressure = !regressPressure;
                                incPressure = -incPressure;
                                double TMP = startPressure;
                                startPressure = stopPressure;
                                stopPressure = TMP;
                            }
                            else
                            {
                                break;
                            }
                        }

                        double currTime = (double)time(NULL);
                        myWeather->Pressure += incPressure * (currTime - prevTime);

                        //printf("PressureChangeSpeed: \t%.5f mbar/second\r\n", incPressure);
                        //printf("Gradient: \t%.5f per %.3f seconds \r\n", incPressure * ((double)currTime - (double)prevTime), (double)currTime - (double)prevTime);

                        prevTime = currTime;
                        myWeather->WindSpeed = avgWindSpeed + (rand() % 4 + 2);
                        {
                            unsigned char* destination;
                            size_t destinationSize;
                            if (SERIALIZE(&destination, &destinationSize
                                , myWeather->DeviceName
                                , myWeather->WindSpeed
                                , myWeather->Pressure
                                , myWeather->Latitude
                                , myWeather->Longitude) != IOT_AGENT_OK)
                            {
                                (void)printf("Failed to serialize\r\n");
                            }
                            else
                            {
                                IOTHUB_MESSAGE_HANDLE messageHandle = IoTHubMessage_CreateFromByteArray(destination, destinationSize);
                                if (messageHandle == NULL)
                                {
                                    printf("unable to create a new IoTHubMessage\r\n");
                                }
                                else
                                {
                                    if (IoTHubClient_LL_SendEventAsync(iotHubClientHandle, messageHandle, sendCallback, (void*)++seqNumber) != IOTHUB_CLIENT_OK)
                                    {
                                        printf("failed to hand over the message to IoTHubClient");
                                    }
                                    else
                                    {
                                        printf("IoTHubClient accepted the message for delivery\r\n");
                                    }

                                    IoTHubMessage_Destroy(messageHandle);
                                }
                                free(destination);
                            }
                        }

                        /* wait for commands */
                        for (int i = 0; i < eventPeriodMilliseconds / 100; ++i)
                        {
                            IoTHubClient_LL_DoWork(iotHubClientHandle);
                            ThreadAPI_Sleep(100);
                        }
                        ThreadAPI_Sleep(eventPeriodMilliseconds % 100);
                    }

                }

                DESTROY_MODEL_INSTANCE(myWeather);
            }
            IoTHubClient_LL_Destroy(iotHubClientHandle);
        }
        serializer_deinit();
    }
}
