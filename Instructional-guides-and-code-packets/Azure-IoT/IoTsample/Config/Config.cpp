#include "Config.h"

//static const Config * _config = NULL;

void ReleaseConfig(CONFIG_HANDLE conf)
{
    delete reinterpret_cast<const Config *>(conf);
}

CONFIG_HANDLE InitConfig(const char * configFilePath)
{
    try
    {
        return new Config(configFilePath);
    }
    catch (...)
    {
        return NULL;
    }
}

bool GetBooleanParameterValue(CONFIG_HANDLE conf, const char * parameterName, bool defaultValue)
{
    if (NULL == conf)
    {
        return defaultValue;
    }
    try
    {
        const Config * config = reinterpret_cast<const Config *>(conf);
        const ConfigParameter & param = (*config)[parameterName];
        return param.AsBool();
    }
    catch (...)
    {
        return defaultValue;
    }
}

double GetDoubleParameterValue(CONFIG_HANDLE conf, const char * parameterName, double defaultValue)
{
    if (NULL == conf)
    {
        return 0.0;
    }
    try
    {
        const Config * config = reinterpret_cast<const Config *>(conf);
        const ConfigParameter & param = (*config)[parameterName];
        return param.AsDouble();
    }
    catch (...)
    {
        return defaultValue;
    }
}


const char * GetStringParameterValue(CONFIG_HANDLE conf, const char * parameterName, const char * defaultValue)
{
    if (NULL == conf)
    {
        return NULL;
    }
    try
    {
        const Config * config = reinterpret_cast<const Config *>(conf);
        const ConfigParameter & param = (*config)[parameterName];
        return param.AsString().c_str();
    }
    catch(...)
    {
        return defaultValue;
    }
}

