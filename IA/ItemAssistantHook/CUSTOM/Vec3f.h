#pragma once
// Definition found in Engine.dll

#if defined(_AMD64_)
struct Vec3f {
	// We want bytes 8-20, ignoring the first 4, keeping 4-8 as a sector Id
	float unknown2, unknown, x, y, z;
};
#else
struct Vec3f {
	float unknown, x, y, z;
};

#endif