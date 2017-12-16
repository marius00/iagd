#ifndef AODatabaseParser_h__
#define AODatabaseParser_h__

#include <boost/smart_ptr.hpp>
#include <boost/filesystem.hpp>
#include <fstream>
#include <shared/AODB.h>
#include <string>


class AODatabaseParser
{
public:
    struct Exception : public std::exception {
        Exception(std::string const& what) : std::exception(what.c_str()) {}
    };

    /// Creates a new AO Database parser for the specified set of files.
    AODatabaseParser(std::vector<std::string> const& aodbfiles);
    ~AODatabaseParser();

    /// Retrieves the item at the specified offset.
    boost::shared_ptr<ao_item> GetItem(unsigned int offset) const;

protected:
    typedef std::pair<uintmax_t, std::string> FileOffsetAndStringPair;

    void EnsureFileOpen(uintmax_t offset) const;
    FileOffsetAndStringPair GetFileFromOffset(uintmax_t offset) const;
    void OpenFileFromOffset(uintmax_t offset) const;

private:
    typedef std::map<uintmax_t, std::string> FileOffsetToStringMap;

    FileOffsetToStringMap m_file_offsets;
    mutable uintmax_t m_current_file_offset;
    mutable uintmax_t m_current_file_size;
    mutable std::ifstream m_file;
    boost::shared_array<char> m_buffer;
};

#endif // AODatabaseParser_h__
