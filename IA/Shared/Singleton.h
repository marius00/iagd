#pragma once

/// Use this macro in the header of the class you wish to make a singleton.
#define SINGLETON(classname)            \
    private:                            \
        classname();                    \
        classname(const classname &);   \
    public:                             \
        static classname & instance();

/// Use this macro in the cpp file of the class you wish to make a singleton.
#define SINGLETON_IMPL(classname)                       \
    classname& classname::instance()                    \
    {                                                   \
        static classname* s_instance = new classname(); \
        return *s_instance;                             \
    }


namespace Shared
{
    template<typename T>
    class Singleton
    {
    public:
        /// Unique point of access
        static T* Instance()
        {
            if (!ms_instance)
            {
                ms_instance = new T;
            }
            return ms_instance;
        }

        static void DestroyInstance()
        {
            delete ms_instance;
            ms_instance = NULL;
        }

    private:
        /// Prevent clients from creating a new singleton
        Singleton()
        {
        }

        /// Prevent clients from creating a copy of the singleton
        Singleton(const Singleton<T>&)
        {
        }

        static T* ms_instance;
    };

    /// Static class member initialisation.
    template <typename T> T* Singleton<T>::ms_instance = NULL;

}   // namespace
