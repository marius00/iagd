#include "StdAfx.h"
#include "AODatabaseIndex.h"

#include <boost/iostreams/device/mapped_file.hpp>

using namespace boost::iostreams;


AODatabaseIndex::AODatabaseIndex(std::string const& index_file, std::set<ResourceType>& types)
{
    ReadIndexFile(index_file, types);
}


AODatabaseIndex::~AODatabaseIndex()
{
}


std::vector<unsigned int> AODatabaseIndex::GetOffsets(ResourceType type) const
{
    std::vector<unsigned int> retval;

    ResourceTypeMap::const_iterator resourceIt = m_record_index.find(type);
    if (resourceIt != m_record_index.end())
    {
        for (IdOffsetMap::const_iterator it = resourceIt->second.begin(); it != resourceIt->second.end(); ++it)
        {
            retval.insert(retval.end(), it->second.begin(), it->second.end());
        }
        std::sort(retval.begin(), retval.end());
    }

    return retval;
}

void AODatabaseIndex::ReadIndexFile(std::string filename, std::set<ResourceType>& types)
{
    mapped_file_source index_file(filename);
    mapped_file_source::iterator current_pos = index_file.begin();

    // Extract some info from the header
    // Info found by tdb @ AODevs.
    unsigned int last_offset = *(unsigned int*)current_pos;
    unsigned int block_size = *(unsigned int*)(current_pos + 0x0c);
    unsigned int data_end = *(unsigned int*)(current_pos + 0x08);
    unsigned int data_start = *(unsigned int*)(current_pos + 0x48);

    // Skip to data_start
    current_pos = index_file.begin() + data_start;

	int iter = 0;
    while (current_pos + block_size < index_file.end())
    {
        unsigned int next_block = ReadIndexBlock(current_pos, std::min<const char*>(index_file.end(), current_pos + block_size), types);
		iter++;
        if (next_block > 0)
        {
            current_pos = index_file.begin() + next_block;
        }
        else
        {
            break;
        }
    }
}

unsigned int AODatabaseIndex::ReadIndexBlock(const char* pos, const char* end, std::set<ResourceType>& types)
{
    // Index blocks are stored in a double linked list. 
    // Extract pointer to next block.
    unsigned int next_block = *(unsigned int*)pos;
    // Extract pointer to previous block.
    unsigned int prev_block = *(unsigned int*)(pos + 4);

    // Number of active record indexes in this block
    unsigned short count = *(unsigned short*)(pos + 8);

    // Skip block header
    pos += 0x12 + 10;
    
    const unsigned int* int_pos = (unsigned int*)pos;
    while (count-- > 0 && (const char*)(int_pos + 2) < end)
    {
		unsigned int ignored = *int_pos;
		++int_pos;	
        unsigned int offset = *int_pos;
        ++int_pos;
        unsigned int resource_type = *int_pos;
        ++int_pos;
        unsigned int resource_id = *int_pos;
        ++int_pos;

        resource_type = _byteswap_ulong(resource_type);
        resource_id = _byteswap_ulong(resource_id);

		
        if (types.empty() || types.find((ResourceType)resource_type) != types.end()) {
            m_record_index[(ResourceType)resource_type][resource_id].insert(offset);
        }
    }

	
    return next_block;
}
