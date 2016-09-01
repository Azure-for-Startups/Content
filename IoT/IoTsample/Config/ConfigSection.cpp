#include "ConfigSection.h"

#include <fstream>
#include <iostream>

ConfigSection::ConfigSection(const ConfigSection &aSection)
  :mMapOfParameters(aSection.mMapOfParameters),
   mFilePath(aSection.mFilePath),
   mSections(aSection.mSections)
{
}

ConfigSection::ConfigSection(const std::string & aFilePath)
  :mFilePath(aFilePath)
{
  std::fstream lConfigFile;
  lConfigFile.open(mFilePath.c_str(), std::ios::in);
  if(lConfigFile.good())
  {
    try
    {
      char lInputString[1024];
      while(lConfigFile.getline(lInputString, sizeof(lInputString)))
      {
        if('#' != lInputString[0])
        {
          ParseInputString(lInputString);
        }
      }
    }
    catch(...)
    {
      lConfigFile.close();
    }
    lConfigFile.close();
  }
  else
  {
      throw "Cannot open configuration file.";
  }
}

static void RemoveCR(std::string & aString)
{
  std::string::size_type lPos;
  while(std::string::npos != (lPos = aString.find('\r')))
  {
    aString.replace(lPos, 1, " ");
  }
  aString = Trim(aString);
}

void ConfigSection::ParseInputString(const char * aInputString)
{
  if(NULL != aInputString && strlen(aInputString) > 3)
  {
    std::string lInputString(aInputString);
    RemoveCR(lInputString);
    size_t lPos = lInputString.find('=');
    if(std::string::npos != lPos && lPos > 0)
    {
      CaseInsensitiveStr lName(Trim(lInputString.substr(0, lPos)));
      ConfigParameter    lValue(Trim(lInputString.substr(lPos + 1)));
      
      if(0 != lName.length() && 0 != lValue.length())
      {
        if('@' == lValue[0])
        {
          AddNewSection(lName, lValue.substr(1));
        }
        else
        {
          mMapOfParameters.insert(tNameValueMap::value_type(lName, lValue));
        }
      }
    }
  }
}

const ConfigParameter & ConfigSection::operator[](const std::string & aParameterName) const
{
  tNameValueMap::const_iterator lFound = mMapOfParameters.find(aParameterName);
  if(mMapOfParameters.end() == lFound)
  {
      throw "Parameter is not found.";
  }
  else
  {
    return lFound->second;
  }
}

const ConfigSection & ConfigSection::Section(const std::string & aSectionName) const
{
  tConfigSectionMap::const_iterator lFound = mSections.find(aSectionName);
  if(mSections.end() == lFound)
  {
      throw "Section is not found.";
  }
  else
  {
    return lFound->second;
  }
}

void ConfigSection::AddNewSection(const CaseInsensitiveStr & aSectionName, const std::string & aFileName)
{
  //try
  {
    mSections.insert(tConfigSectionMap::value_type(aSectionName, ConfigSection(MakeSectionFilePath(aFileName))));
  }
  //catch(xGenericException & ex)
  //{
  //  //tLog::Singletone().Error(ex, "Cannot add section \"%s\" from \"%s\" file.", aSectionName.c_str(), aFileName.c_str());
  //}
  //catch(std::exception & ex)
  //{
  //  //tLog::Singletone().Error("Cannot add section \"%s\" from \"%s\" file. %s", 
  //    aSectionName.c_str(), aFileName.c_str(), ex.what());
  //}
}

static void ReplaceBackSlash(std::string & aString)
{
    std::string::size_type lPos;
    while (std::string::npos != (lPos = aString.find('\\')))
    {
        aString.replace(lPos, 1, "/");
    }
}

static std::string RetrivePath(const std::string & aFilePath)
{
    std::string::size_type lPos = aFilePath.rfind('/');
    if (std::string::npos != lPos)
    {
        return aFilePath.substr(0, lPos);
    }
    else
    {
        //return "./";
        return "";
    }
}


const std::string ConfigSection::MakeSectionFilePath(const std::string & aRelativeFilePath)
{
  std::string lResult(aRelativeFilePath);
  ReplaceBackSlash(lResult);
  if('.' == lResult[0])
  {
    lResult = RetrivePath(mFilePath) + lResult.substr(1);
  }
  return lResult;
}

const ConfigParameter & ConfigSection::GetValue(const std::string & aParameterName, const ConfigParameter & aDefaultValue) const
{
  try
  {
    return (*this)[aParameterName];
  }
  catch(...)
  {
    return aDefaultValue;
  }
}
