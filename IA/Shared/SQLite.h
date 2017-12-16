#ifndef SQLITE_H
#define SQLITE_H

#include <shared/UnicodeSupport.h>
#include <boost/smart_ptr.hpp>
#include <vector>
#include <shared/IDB.h>

// Forward declarations
struct sqlite3;

namespace sqlite
{

    class Db;
    class Table;

    typedef boost::shared_ptr<Db> DbPtr;
    typedef boost::shared_ptr<Table> TablePtr;


    class Table
        : public ITable
    {
    public:
        Table(int nrow, int ncol, char **result);

        //std::vector<std::string> const& Headers() const { return m_headers; }

        virtual std::string Headers( unsigned int col ) const;

        //std::vector<std::string> const& Data() const { return m_data; }
        virtual std::string Data(unsigned int row, unsigned int col) const;

        virtual size_t Columns() const;
        virtual size_t Rows() const;

    private:
        std::vector<std::string> m_headers;
        std::vector<std::string> m_data;
    };


    class Db
        : public IDB
    {
    public:
        struct QueryFailedException : public std::exception
        {
            QueryFailedException(std::tstring const& msg) : std::exception(to_ascii_copy(msg).c_str()) {}
        };

        Db(std::ostream &log);
        virtual ~Db();

        virtual bool Init(std::tstring const& filename = _T("init.db"));
        virtual void Term();

        virtual bool Exec(std::wstring const& sql) const;
        virtual bool Exec(std::string const& sql) const;

        virtual ITablePtr ExecTable(std::wstring const& sql) const;
        virtual ITablePtr ExecTable(std::string const& sql) const;

        virtual void Begin() const;
        virtual void Commit() const;
        virtual void Rollback() const;

    private:
        sqlite3 *m_pDb;
        std::ostream &m_log;
    };

}  // namespace SQLite


#endif // SQLITE_H
