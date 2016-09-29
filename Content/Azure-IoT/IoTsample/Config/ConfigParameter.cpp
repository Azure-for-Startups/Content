#include "ConfigParameter.h"
#include <errno.h>

//================== tConfigParameter ==========================
//allowed bolean values:
const CaseInsensitiveStr ConfigParameter::cTrue("True");
const CaseInsensitiveStr ConfigParameter::cFalse("False");
const CaseInsensitiveStr ConfigParameter::cYes("Yes");
const CaseInsensitiveStr ConfigParameter::cNo("No");
const CaseInsensitiveStr ConfigParameter::cY("Y");
const CaseInsensitiveStr ConfigParameter::cN("N");
const CaseInsensitiveStr ConfigParameter::c1("1");
const CaseInsensitiveStr ConfigParameter::c0("0");

bool ConfigParameter::AsBool() const
{
  CaseInsensitiveStr stringValue(*this); 
  if(stringValue == cTrue || stringValue == cYes || stringValue == cY || stringValue == c1)
  {
    return true;
  }
    return false;
}


int ConfigParameter::CheckConversion(int retValue) const
{
  if(errno == ERANGE)
  {
      return 0;
  }
  return retValue;
}
//Int64 ConfigParameter::CheckConversion(Int64 retValue) const
//{
//  if(errno == ERANGE)
//  {
//    return 0
//  }
//  return retValue;
//}
double ConfigParameter::CheckConversion(double retValue) const
{
  if(errno == ERANGE)
  {
      return 0.0;
  }
  return retValue;
}
