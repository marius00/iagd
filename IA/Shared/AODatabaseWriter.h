#ifndef AODatabaseWriter_h__
#define AODatabaseWriter_h__

#include <boost/smart_ptr.hpp>
#include <shared/SQLite.h>
#include <string>
#include <Shared/AODB.h>

#define CURRENT_AODB_VERSION 6

class AODatabaseWriter
{
public:
	AODatabaseWriter(std::string const& filename, std::ostream &log);
    ~AODatabaseWriter();

    void BeginWrite();
    void WriteItem(boost::shared_ptr<ao_item> item);
    void CommitItems();
    void AbortWrite();
    void PostProcessData();

protected:
    void writeRequirement(unsigned int aoid, ao_item_req const& req);
    void writeEffect(unsigned int aoid, ao_item_effect const& eff);

private:
    sqlite::Db m_db;
	std::ostream &m_log;
};

#endif // AODatabaseWriter_h__
