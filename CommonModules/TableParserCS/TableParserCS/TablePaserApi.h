// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the RTNGINEREMOTEAPI_EXPORTS
// symbol defined on the command line. this symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// RTNGINEREMOTEAPI_API functions as being imported from a DLL, wheras this DLL sees symbols
// defined with this macro as being exported.
#if 1

#ifdef TABLEPASERAPI_EXPORTS
#define TABLEPASERAPI_DLL __declspec(dllexport)
#else
#pragma comment(lib, "TablePaserAPI.lib")
#define TABLEPASERAPI_DLL __declspec(dllimport)
#endif

#else

#define TABLEPASERAPI_DLL
#endif

//////////////////////////////////////////////////////////////////////////
//



extern "C"{	
	

	TABLEPASERAPI_DLL int tpOpenTableFile(char *ePathFile, char *eTableType) ; 
	TABLEPASERAPI_DLL int tpSaveTableFile(char *ePathFile) ; 
	TABLEPASERAPI_DLL int tpGetVarCount();
	TABLEPASERAPI_DLL int tpMoveFirstVar();
	TABLEPASERAPI_DLL int tpMoveNextVar();
	TABLEPASERAPI_DLL int tpGetVarNext(int *eVarIndex, int *eVarType, char *eVarName, char *ekey);
	TABLEPASERAPI_DLL int tpMovePrevVar();
	TABLEPASERAPI_DLL int tpGetVarPrev(int *eVarIndex, int *eVarType, char *eVarName, char *ekey);
	TABLEPASERAPI_DLL int tpMoveLastVar();
	TABLEPASERAPI_DLL int tpGetCurVarInfo(int *eVarIndex, int *eVarType, char *eVarName, char *ekey);
}
