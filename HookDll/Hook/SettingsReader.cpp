#include "SettingsReader.h"
#include <boost/property_tree/ptree.hpp>                                        
#include <boost/property_tree/json_parser.hpp>       
#include <boost/optional/optional.hpp>

std::wstring GetIagdFolder();
void LogToFile(const wchar_t* message);
void LogToFile(std::wstring message);


int SettingsReader::GetStashTabToLootFrom() {
	boost::property_tree::wptree loadPtreeRoot;

	const auto settingsJson = GetIagdFolder() + L"settings.json";
	std::wifstream json(settingsJson);

	boost::property_tree::read_json(json, loadPtreeRoot);
	auto child = loadPtreeRoot.get_child_optional(L"local.stashToLootFrom");
	if (!child)
	{
		LogToFile(L"No \"loot from\" configuration found, defaulting to last stash tab");
		return 0;
	}

	const int stashToLootFrom = loadPtreeRoot.get<int>(L"local.stashToLootFrom");


	if (stashToLootFrom == 0) {
		LogToFile(L"Configured to loot from last stash tab");

	}
	else {
		LogToFile(L"Configured to loot from tab: " + std::to_wstring(stashToLootFrom));
	}

	return stashToLootFrom;
}

int SettingsReader::GetStashTabToDepositTo() {
	boost::property_tree::wptree loadPtreeRoot;

	const auto settingsJson = GetIagdFolder() + L"settings.json";
	std::wifstream json(settingsJson);

	boost::property_tree::read_json(json, loadPtreeRoot);
	auto child = loadPtreeRoot.get_child_optional(L"local.stashToDepositTo");
	if (!child)
	{
		LogToFile(L"No \"deposit to\" configuration found, defaulting to second-to-last stash tab");
		return 0;
	}

	const int stashToDepositTo= loadPtreeRoot.get<int>(L"local.stashToDepositTo");


	if (stashToDepositTo == 0) {
		LogToFile(L"Configured to deposit to last stash tab");

	}
	else {
		LogToFile(L"Configured to deposit to tab: " + std::to_wstring(stashToDepositTo));
	}

	return stashToDepositTo;
}

bool SettingsReader::GetPreferLegacyMode() {
	boost::property_tree::wptree loadPtreeRoot;

	const auto settingsJson = GetIagdFolder() + L"settings.json";
	std::wifstream json(settingsJson);


	boost::property_tree::read_json(json, loadPtreeRoot);
	auto child = loadPtreeRoot.get_child_optional(L"local.preferLegacyMode");
	if (!child)
	{
		LogToFile(L"Legacy mode: No configuration found, defaulting to standard mode");
		return true;
	}

	const bool isLegacyMode = loadPtreeRoot.get<bool>(L"local.preferLegacyMode");
	LogToFile(std::wstring(L"Legacy mode: ") + (isLegacyMode ? L"True" : L"False"));

	return isLegacyMode != 1;
}


bool SettingsReader::GetIsGrimDawnParsed() {
	boost::property_tree::wptree loadPtreeRoot;

	const auto settingsJson = GetIagdFolder() + L"settings.json";
	std::wifstream json(settingsJson);


	boost::property_tree::read_json(json, loadPtreeRoot);
	auto child = loadPtreeRoot.get_child_optional(L"local.isGrimDawnParsed");
	if (!child)
	{
		LogToFile(L"GrimDawnParsed: No configuration found, defaulting to NOT parsed");
		return false;
	}

	const bool isGdParsed = loadPtreeRoot.get<bool>(L"local.isGrimDawnParsed");
	LogToFile(std::wstring(L"Grim Dawn parsed: ") + (isGdParsed ? L"True" : L"False"));

	return isGdParsed;
}
