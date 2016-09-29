#ifndef TSTRINGS_H
#define TSTRINGS_H

#include <string>

#ifdef _WIN32
  #define strcasecmp _stricmp
#endif

class CaseInsensitiveStr : public std::string
{
  public:
  CaseInsensitiveStr()
  {}
  CaseInsensitiveStr(const std::string & a) : std::string(a)
  {}
  CaseInsensitiveStr(const char * a) : std::string(a)
  {}
}; 

inline bool operator== (const CaseInsensitiveStr & a1, const CaseInsensitiveStr & a2)
{
  return strcasecmp(a1.c_str(), a2.c_str()) == 0;
}
inline bool operator< (const CaseInsensitiveStr & a1, const CaseInsensitiveStr & a2)
{
  return strcasecmp(a1.c_str(), a2.c_str()) < 0;
}

inline std::string Trim(const std::string & aString)
{
  std::string lString(aString);
  while(lString.length() > 0 && lString[0] == ' ')
  {
    lString.erase(0, 1);
  }
  while(lString.length() > 0 && lString[lString.length()-1] == ' ')
  {
    lString.erase(lString.length()-1);
  }
  return lString;
}

#endif
