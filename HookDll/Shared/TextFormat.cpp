#include "StdAfx.h"
#include "TextFormat.h"

TextFormat::TextFormat(int width, char* delimiter)
    : m_width(width)
    , m_delimeter(*delimiter)
{
}


TextFormat::~TextFormat()
{
}

std::tstring TextFormat::FormatLongToString(long l) const
{
	char s[128];
	long i, n, sign;

    if ((sign = l) < 0)					/* record sign */
        l = -l;							/* make n positive */
    i = 0;
	n = 0;
    do {								/* generate digits in reverse order */
		if (n == (m_width))
		{
			n = 0;
			s[i++] = m_delimeter;
		}

		n++;
		s[i++] = l % 10 + '0';			/* get next digit */

    } while ((l /= 10) > 0);		/* delete it */

    if (sign < 0)
        s[i++] = '-';
    s[i] = '\0';

	i = n = 0;
    char c;

    for (i = 0, n = strlen(s)-1; i<n; i++, n--)
	{
        c = s[i];
        s[i] = s[n];
        s[n] = c;
	}
	return boost::lexical_cast<std::tstring>(s);
}

std::tstring TextFormat::FormatLongLongToString(unsigned __int64 l) const
{
	char s[128];
	long i, n;

	//if ((sign = l) < 0)					/* record sign */
	//	l = -l;							/* make n positive */

	i = 0;
	n = 0;
	do {								/* generate digits in reverse order */
		if (n == (m_width))
		{
			n = 0;
			s[i++] = m_delimeter;
		}

		n++;
		s[i++] = l % 10 + '0';			/* get next digit */

	} while ((l /= 10) > 0);		/* delete it */


	s[i] = '\0';

	i = n = 0;
	char c;

	for (i = 0, n = strlen(s)-1; i<n; i++, n--)
	{
		c = s[i];
		s[i] = s[n];
		s[n] = c;
	}
	return boost::lexical_cast<std::tstring>(s);
}
