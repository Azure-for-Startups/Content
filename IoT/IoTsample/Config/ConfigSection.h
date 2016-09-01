#ifndef ConfigSectionH
#define ConfigSectionH

#include "ConfigParameter.h"
#include <map>


class ConfigSection
{
public:
  ConfigSection(const ConfigSection &);
  const ConfigParameter & operator[](const std::string & aParameterName) const;
  const ConfigParameter & GetValue(const std::string & aParameterName, const ConfigParameter &  aDefaultValue) const;
  const ConfigSection   & Section(const std::string & aSectionName) const;
protected:
  ConfigSection(const std::string & aFilePath);
private:
  const ConfigSection & operator=(const ConfigSection &);
  void ParseInputString(const char * aInputString);
  void AddNewSection(const CaseInsensitiveStr & aSectionName, const std::string & aFileName);
  const std::string MakeSectionFilePath(const std::string & aRelativeFilePath);

  typedef std::map<CaseInsensitiveStr, ConfigSection> tConfigSectionMap;
  tConfigSectionMap mSections;

  typedef std::map<CaseInsensitiveStr, ConfigParameter> tNameValueMap;
  tNameValueMap mMapOfParameters;
  const std::string mFilePath;
};


#endif //ConfigSectionH
