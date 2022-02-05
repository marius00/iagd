#include "SettingsReader.h"
#include <boost/property_tree/ptree.hpp>                                        
#include <boost/property_tree/json_parser.hpp>       
#include <boost/optional/optional.hpp>

std::wstring GetIagdFolder();
void LogToFile(const wchar_t* message);
void LogToFile(std::wstring message);


int SettingsReader::getStashTabToLootFrom() {
	boost::property_tree::wptree loadPtreeRoot;

	const auto settingsJson = GetIagdFolder() + L"settings.json";
	std::wifstream json(settingsJson);

	boost::property_tree::read_json(json, loadPtreeRoot);
	auto child = loadPtreeRoot.get_child_optional(L"local.stashToLootFrom");
	if (!child)
	{
		return 0;
	}

	const int stashToLootFrom = loadPtreeRoot.get<int>(L"local.stashToLootFrom");


	if (stashToLootFrom == 0) {
		LogToFile(L"Configured to loot from last stash tab");

	} else {
		LogToFile(L"Configured to loot from tab: " + std::to_wstring(stashToLootFrom));
	}

	return stashToLootFrom;
}

bool SettingsReader::getInstalootActive() {
	boost::property_tree::wptree loadPtreeRoot;

	const auto settingsJson = GetIagdFolder() + L"settings.json";
	std::wifstream json(settingsJson);


	boost::property_tree::read_json(json, loadPtreeRoot);
	auto child = loadPtreeRoot.get_child_optional(L"local.instalootDisabled");
	if (!child)
	{
		LogToFile(L"InstalootDisabled: No configuration found, defaulting to enabled");
		return false;
	}
	
	const bool instalootDisabled = loadPtreeRoot.get<int>(L"local.instalootDisabled") == 1;
	LogToFile(std::wstring(L"InstalootDisabled: ") + (instalootDisabled ? L"True" : L"False"));

	return instalootDisabled != 1;
}
