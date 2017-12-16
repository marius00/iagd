#ifndef IDB_H
#define IDB_H

#include <shared/ITable.h>

namespace sqlite
{

    struct IDB
    {
        virtual ~IDB() {}

        /// Initialize the DB using the specified filename.
        virtual bool Init(std::tstring const& filename) = 0;

        /// Close the database.
        virtual void Term() = 0;

        /// Lock database for access.
        //virtual void Lock() = 0;

        /// Release database lock.
        //virtual void UnLock() = 0;

        /// Execute query and retrieve results as a table.
        virtual ITablePtr ExecTable(std::wstring const& sql) const = 0;

        /// Execute query and retrieve results as a table.
        virtual ITablePtr ExecTable(std::string const& sql) const = 0;

        /// Execute query but dot not return result. Returns false if query failed.
        virtual bool Exec(std::wstring const& sql) const = 0;

        /// Execute query but dot not return result. Returns false if query failed.
        virtual bool Exec(std::string const& sql) const = 0;

        /// Start transaction.
        virtual void Begin() const = 0;

        /// Complete transaction and commit changes to the DB
        virtual void Commit() const = 0;

        /// Abort transaction and revert changes.
        virtual void Rollback() const = 0;
    };

    typedef boost::shared_ptr<IDB> IDBPtr;

}


#endif // IDB_H
