#include "stdafx.h"
#include "AODB.h"
#include <malloc.h>
#include <string>


ao_item_effect::ao_item_effect()
    : hook(0)
    , type(0)
    , hits(0)
    , delay(0)
    , target(0)
{
}


ao_item::ao_item()
    : metatype(' ')
    , aoid(0)
    , flags(0)
    , props(0)
    , ql(0)
    , icon(0)
    , type(0)
    , slot(0)
    , defslot(0)
    , value(0)
    , tequip(0)
    , crystal(0)
    , ncu(0)
    , nanocost(0)
    , school(0)
    , strain(0)
    , duration(0)
    , multim(0)
    , multir(0)
    , tburst(0)
    , tfullauto(0)
    , clip(0)
    , dcrit(0)
    , atype(0)
    , mareq(0)
    , tattack(0)
    , trecharge(0)
    , dmax(0)
    , dmin(0)
    , range(0)
    , dtype(0)
    , initskill(0)
{

}