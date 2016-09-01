#ifndef ConfigH
#define ConfigH

#ifdef __cplusplus
#include <string>
#include "ConfigSection.h"

class Config : public ConfigSection
{
public:
    Config(const std::string & configFilePath) : ConfigSection(configFilePath)
    {
    }
private:
    Config(const Config &);
    const Config & operator=(const Config &);
};
#else

#endif

#ifdef __cplusplus
extern "C" {
#endif
    typedef const void * CONFIG_HANDLE;
    void ReleaseConfig(CONFIG_HANDLE);
    CONFIG_HANDLE InitConfig(const char * configFilePath);
    const char * GetStringParameterValue(CONFIG_HANDLE conf, const char * parameterName, const char * defaultValue);
    double GetDoubleParameterValue(CONFIG_HANDLE conf, const char * parameterName, double defaultValue);
    bool GetBooleanParameterValue(CONFIG_HANDLE conf, const char * parameterName, bool defaultValue);

#ifdef __cplusplus
}
#endif



#endif //ConfigH
