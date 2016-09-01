#ifndef ConfigParameterH
#define ConfigParameterH

//#include "AdvTools/Types.h"
//#include "AdvTools/DateTime.h"
#include "Strings.h"

class ConfigParameter : public std::string
{
public:
  ConfigParameter() : std::string()
  {
  }
  ConfigParameter(const char * aStr) : std::string(aStr)
  {
  }
  ConfigParameter(const ConfigParameter & aParam) : std::string(aParam)
  {
  }
  ConfigParameter(const std::string & aStr) : std::string(aStr)
  {
  }
  
  const std::string & AsString() const
  {
    return *this;
  }
  int AsInt() const
  {
    return CheckConversion(atoi(this->c_str()));
  }
  unsigned int AsUInt() const
  {
    return static_cast<unsigned int>(atoi(this->c_str()));
  }
  //Int64 AsInt64() const
  //{
  //  return CheckConversion(atoInt64(this->c_str()));
  //}
  //UInt64 AsUInt64() const
  //{
  //  return static_cast<UInt64>(AsInt64());
  //}
  double AsDouble() const
  {
    return CheckConversion(atof(this->c_str()));
  }
  bool AsBool() const;

  //tDateTime AsDateTime() const
  //{
  //  return tDateTime::Parse(*this);
  //}
  
  //TODO:
  // tTimeSpan AsTimeSpan() const;
private:
  int CheckConversion(int aRetValue) const;
  //Int64 CheckConversion(Int64 aRetValue) const;
  double CheckConversion(double aRetValue) const;

  //allowed bolean values:
  static const CaseInsensitiveStr cTrue;
  static const CaseInsensitiveStr cFalse;
  static const CaseInsensitiveStr cYes;
  static const CaseInsensitiveStr cNo;
  static const CaseInsensitiveStr cY;
  static const CaseInsensitiveStr cN;
  static const CaseInsensitiveStr c1;
  static const CaseInsensitiveStr c0;
};



#endif //ConfigParameterH
