#ifndef ITABLE_H
#define ITABLE_H

namespace sqlite
{

    struct ITable
    {
        virtual ~ITable() {}

        virtual std::string Headers(unsigned int col) const = 0;
        virtual std::string Data(unsigned int row, unsigned int col) const = 0;
        virtual size_t Columns() const = 0;
        virtual size_t Rows() const = 0;
    };

    typedef boost::shared_ptr<ITable> ITablePtr;

}

#endif // ITABLE_H
