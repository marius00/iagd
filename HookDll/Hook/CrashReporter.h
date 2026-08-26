#pragma once
#include <windows.h>
#include <stdint.h>

/// Self-reporting crash diagnostics for the injected hook.
///
/// Evidence has to come from users, who cannot be asked to install a debugger or edit the registry. This
/// writes a symbolisable crash report, a minidump and a snapshot of the hook log into the IAGD folder at the
/// moment the game faults, leaving nothing for the player to do but send the files.
///
/// Everything here runs on a thread that is already faulting, so the handler:
///   - resolves its output folder once during Install(), never during the fault
///   - allocates nothing, takes no lock, and does not touch HookLog (whose mutex the faulting thread may hold)
///   - writes through raw CreateFile/WriteFile only
///
/// Note that a heap-corruption or stack-cookie failure raises __fastfail, which bypasses vectored handlers
/// entirely. If users keep crashing while no report is ever produced, that absence is itself the finding: it
/// says the fault is a fail-fast rather than an access violation.
namespace CrashReporter {
	/// Registers the vectored exception handler. Safe to call more than once.
	void Install();

	/// Unregisters the handler. Must happen before the DLL unloads or the process jumps into freed memory.
	void Uninstall();

	/// Records one line of "what was the hook doing" into a lock-free ring, replayed in the crash report.
	///
	/// Cheap by design: no allocation, no lock, one interlocked increment. `tag` must have static storage
	/// duration (a string literal) because the ring stores the pointer rather than a copy, and the report
	/// dereferences it after the fault.
	void Note(const char* tag, uint64_t a = 0, uint64_t b = 0);
}
