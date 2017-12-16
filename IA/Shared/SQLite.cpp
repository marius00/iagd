#include "stdafx.h"
#include "SQLite.h"
#include <sqlite3.h>
#include <iostream>

namespace sqlite
{

    Db::Db(std::ostream& log)
        : m_pDb(NULL)
        , m_log(log) {}


    Db::~Db()
    {
        sqlite3_close(m_pDb);
    }


    bool Db::Init(std::tstring const& filename)
    {
        int err = sqlite3_open(to_ascii_copy(filename).c_str(), &m_pDb);
        if (SQLITE_OK != err)
        {
            m_log << "Db::Init: failed to open file [" << to_ascii_copy(filename) << "]. Error " << err << ", " << sqlite3_errmsg(m_pDb) << std::endl;
            sqlite3_close(m_pDb);
            m_pDb = NULL;
            return false;
        }
        return true;
    }


    void Db::Term()
    {
        if (m_pDb != NULL)
        {
            sqlite3_close(m_pDb);
            m_pDb = NULL;
        }
    }


    ITablePtr Db::ExecTable(std::wstring const& sql) const
    {
        return ExecTable(to_utf8_copy(sql));
    }


    ITablePtr Db::ExecTable(std::string const& sql) const
    {
        TablePtr pRes;
        char** result;
        int nrow;
        int ncol;

        int err = sqlite3_get_table(m_pDb,
                                    sql.c_str(),
                                    &result,          /* Result written to a char *[]  that this points to */
                                    &nrow,            /* Number of result rows written here */
                                    &ncol,            /* Number of result columns written here */
                                    NULL);

        if (SQLITE_OK != err)
        {
            assert(false);
            m_log << "Db::ExecTable: Failed to execute query [" << sql << "]. Error: " << err << ", " <<
                sqlite3_errmsg(m_pDb) << std::endl;
            std::tstring msg = STREAM2STR("Query Failed with error code " << err);
            throw QueryFailedException(msg);
        }

        pRes = TablePtr(new Table(nrow, ncol, result));
        sqlite3_free_table(result);

        return pRes;
    }


    bool Db::Exec(std::wstring const& sql) const
    {
        return Exec(to_ascii_copy(sql));
    }


    bool Db::Exec(std::string const& sql) const
    {
        int err = sqlite3_exec(m_pDb, sql.c_str(), NULL, NULL, NULL);
        if (SQLITE_OK != err)
        {
            m_log << "Db::Exec: Failed to execute query [" << sql << "]. Error: " << err << ", " << sqlite3_errmsg(m_pDb) << std::endl;
            return false;
        }
        return true;
    }


    void Db::Begin() const
    {
        Exec(_T("BEGIN TRANSACTION"));
    }


    void Db::Commit() const
    {
        Exec(_T("COMMIT TRANSACTION"));
    }


    void Db::Rollback() const
    {
        Exec(_T("ROLLBACK TRANSACTION"));
    }


    //////////////////////////////////////////////////////////////////////////
    // Table 
    //////////////////////////////////////////////////////////////////////////

    Table::Table(int nrow, int ncol, char** result)
    {
        m_headers.reserve(ncol);
        m_data.reserve(nrow * ncol);

        for (int i = 0; i < ncol; ++i)
        {
            m_headers.push_back(result[i]);   /* First row heading */
        }
        for (int i = 0; i < ncol * nrow; ++i)
        {
            if (result[ncol + i] == NULL)
            {
                m_data.push_back(std::string());
            }
            else
            {
                m_data.push_back(result[ncol + i]);
            }
        }
    }

    std::string Table::Headers(unsigned int col) const
    {
        return m_headers.at(col);
    }

    std::string Table::Data(unsigned int row, unsigned int col) const
    {
        if (row < Rows() && col < Columns())
        {
            return m_data.at(Columns() * row + col);
        }
        return "";
    }


    size_t Table::Columns() const
    {
        return m_headers.size();
    }


    size_t Table::Rows() const
    {
        return m_headers.size() > 0 ? m_data.size() / m_headers.size() : 0;
    }

}  // namespace sqlite
