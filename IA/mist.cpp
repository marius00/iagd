103C8B8C

GAME::DamageAttributeAbsMod::GetText(std::basic_string<ushort,std::char_traits<ushort>,std::allocator<ushort>> &,uint,bool,bool)



      (*(void (__thiscall **)(int, int))(*(_DWORD *)v14 + 116))(v14, a1);
      (*(void (__thiscall **)(int, _DWORD))(*(_DWORD *)v14 + 88))(v14, LODWORD(attrib));
      (*(void (__thiscall **)(int, _DWORD))(*(_DWORD *)v14 + 92))(v14, LODWORD(attrib));
	  
	  103BE388+
	  88				103BE3E0	?ScaleDamage@CombatAttributeDurDamage@GAME@@UAEXM@Z ; GAME::CombatAttributeDurDamage::ScaleDamage(float)
	  92				103BE3E4	?ScaleModifiers@CombatAttributeDurDamage@GAME@@UAEXM@Z ; GAME::CombatAttributeDurDamage::ScaleModifiers(float)
	  116				103BE3FC	?Copy@CombatAttributeDurDamage@GAME@@UAEXPAVCombatAttribute@2@@Z ; GAME::CombatAttributeDurDamage::Copy(GAME::CombatAttribute *)
	  
	  
	  
	  
double __thiscall GAME::DamageAttribute_Physical::GetValueMin(GAME::DamageAttribute_Physical *this, unsigned int num, bool usually_false)	  
  this_ = this;
  v10 = 0.0;
  if ( num && (*((_DWORD *)this + 24) - *((_DWORD *)this + 23)) >> 3 )
  {
    v4 = (__m128d)COERCE_UNSIGNED_INT64((double)(signed int)(num - 1));
    v4.m128d_f64[0] = v4.m128d_f64[0] + *(double *)&qword_103EBC00[(num - 1) >> 31];
    v5 = sub_10323020((__m128i)_mm_cvtpd_ps(v4));
    if ( v5 > ((*((_DWORD *)this_ + 24) - *((_DWORD *)this_ + 23)) >> 3) - 1 )
      v5 = ((*((_DWORD *)this_ + 24) - *((_DWORD *)this_ + 23)) >> 3) - 1;
    v6 = *((_DWORD *)this_ + 23);
    v10 = *(float *)(v6 + 8 * v5);
    if ( usually_false )
    {
      v7 = *(_DWORD *)(v6 + 8 * v5 - 8);
      __asm { lahf }
      if ( !__SETP__(_AH & 0x44, 0) )
        v10 = 0.0;
    }
  }
  return v10;
}