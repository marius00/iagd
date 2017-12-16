#ifndef AODATABASEINDEX_H
#define AODATABASEINDEX_H

#include <map>
#include <set>
#include <Shared/AODB.h>

class AODatabaseIndex
{
public:
    /// Extracts all the specified resource types from the index file. If no 
    /// resource types are specified, all indexes will be extracted.
    AODatabaseIndex(std::string const& index_file, std::set<ResourceType>& types);
    ~AODatabaseIndex();

    /// Returns a sorted array of all the record offsets for a specific resource type.
    std::vector<unsigned int> GetOffsets(ResourceType type) const;

protected:
    void ReadIndexFile(std::string filename, std::set<ResourceType>& types);
    unsigned int ReadIndexBlock(const char* pos, const char* end, std::set<ResourceType>& types);

private:
    typedef std::map<unsigned int, std::set<unsigned int> > IdOffsetMap;
    typedef std::map<ResourceType, IdOffsetMap> ResourceTypeMap;

    // This maps a resource type to a map from resource ID to resource offset in the database file.
    ResourceTypeMap m_record_index;
};

#endif // AODATABASEINDEX_H
