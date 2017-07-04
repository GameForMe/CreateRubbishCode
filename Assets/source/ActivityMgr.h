#pragma once
#include <string>
#include <vector>
#include"Attres.h"
#define ACT_START_MINUTE_FAST	2 //每日活动开始的前两分钟闪UI
#define ACT_START_MINUTE_SLOW	10//每日活动开始的前十分钟闪UI
//特殊活动索引
#define ACT_LUANDOU_INDEX		2 //乱斗会
#define ACT_BOSSFIGHT_INDEX		8 //魔王降世
#define ACT_FIGHTCITY_INDEX		7 //攻城战
#define ACT_TIANLAO_INDEX       12//勇闯天牢
#define ACT_FMB_INDEX			14
#define ACT_BF_INDEX			15
#define ACT_ANSWER_INDEX        16//答题活动
#define isOpenActive(num)       ((num) == ACT_ANSWER_INDEX)

#define MOSHEN_INTERVAL_TIME	10//魔神大厅活动参加的间隔时间
struct DayActivityData
{
	enum eState
	{
		S_Jinxingzhong,
		S_Weikaishi,
		S_Yijieshu,
		S_NotToday,//gx add 2014.2.17攻城战与行会BOSS战区分
		S_Finish  //已完成
	};

	std::string name;
	std::string npcName;
	unsigned int mapID;
	std::string reward;
	std::string time;
	std::string desc;
	eState state;
	std::string level;
	//int level;
	int begin_hour;
	int begin_min;
	int end_hour;
	int end_min;
	int x , y;
	//int actid;//gx add 2013.12.23 记录活动ID
	int item[JiangLiNumber];
	int comeleteNumbercishu;//总次数
	int huoyuedu;
	int weekday;//周几开始
	int id;
	int openLv;
	std::string transfer_pic;
	std::string pic_eff;
	int type;
	int openUI;
	int is_team_show;
	DayActivityData()
	{
		state = S_Weikaishi;
	}
};
struct tag_Act_Broad
{
	int ID;
	int start_year;
	int start_month;
	int start_day;
	int start_hour;
	int end_year;
	int end_month;
	int end_day;
	int end_hour; 
	std::string strText;
};


//沙城战时间表
//正常																		
//1、返还行会资金，竞标资金清零，取消参战状态（开始）
//2、返还行会资金，竞标资金清零，取消参战状态（结束）   竞标（开始）		
//3、竞标（结束）															
//4、决定参战行会（开始）													
//5、决定参战行会（结束）								沙巴克攻城（开始）
//6、沙巴克攻城（结束）														
//7、沙巴克攻城前准备，踢人开始时间
//8、沙巴克攻城前准备，踢人结束时间
//9、沙巴克皇宫进入时间（开始）
//10、沙巴克皇宫进入时间（结束）

//服务器启动
//1、
//2、返还行会资金，竞标资金清零，取消参战状态（结束）		不返还行会资金，竞标资金不清零，取消参战状态（开始）
//3、
//4、不返还行会资金，竞标资金不清零，取消参战状态（结束）	不返还行会资金，竞标资金不清零，不取消参战状态，继续攻城战（开始）
//5、
//6、返还行会资金，竞标资金清零，取消参战状态（开始）		不返还行会资金，竞标资金不清零，不取消参战状态，继续攻城战（结束）

enum e_ActivityType
{
	e_ActivityBaiRiMen = 1,
	e_ActivityLuanDou = 2,
	e_ActivityDuoBao = 3,
	e_ActivityShengYan = 5,
	e_ActivitMoHun = 6,
	e_ActivityShaChengZhan = 7,
	e_ActivityMoShen = 8,
	e_ActivityZuMaGe = 9,
	e_ActivityNianShou = 11,
	e_ActivityTianLao = 12,
	e_ActivityYuYaoGongCheng = 13,
	e_ActivityFengMoBang = 14,
	e_ActivityXueZhanDaoDi = 15,
};

enum enumSBKTimeType
{
	ESTT_1 = 1,
	ESTT_2,
	ESTT_3,
	ESTT_4,
	ESTT_5,
	ESTT_6,
	ESTT_7,
	ESTT_8,
	ESTT_9,
	ESTT_10,
	ESTT_11
};

struct sbkTimePro
{
	BYTE id;
	std::vector<INT> week;
	BYTE hour;
	BYTE min;

	sbkTimePro()
	{
		id = 0;
		week.clear();
		hour = 0;
		min = 0;
	}
};

class ActivityMgr
{
private:
	std::vector<DayActivityData> _allData;
	int _startActIndex;
	int _startActIndexDayActivity;
	int _startActLevelDayActivity;
	bool _bTransmit;//是否传送 
	bool _isTransmitSBK; 
	float _lastSbkTime;
	DWORD _lastMoshenTime;//上次点击魔神大厅活动的时间

	void init_act_data();
	//沙城战时间表
	//正常																		
	//1、返还行会资金，竞标资金清零，取消参战状态（开始）
	//2、返还行会资金，竞标资金清零，取消参战状态（结束）   竞标（开始）		
	//3、竞标（结束）															
	//4、决定参战行会（开始）													
	//5、决定参战行会（结束）								沙巴克攻城（开始）
	//6、沙巴克攻城（结束）														
	//7、沙巴克攻城前准备，踢人开始时间
	//8、沙巴克攻城前准备，踢人结束时间

	//服务器启动
	//1、
	//2、返还行会资金，竞标资金清零，取消参战状态（结束）		不返还行会资金，竞标资金不清零，取消参战状态（开始）
	//3、
	//4、不返还行会资金，竞标资金不清零，取消参战状态（结束）	不返还行会资金，竞标资金不清零，不取消参战状态，继续攻城战（开始）
	//5、
	//6、返还行会资金，竞标资金清零，取消参战状态（开始）		不返还行会资金，竞标资金不清零，不取消参战状态，继续攻城战（结束）
	std::map<BYTE,sbkTimePro>	_sbkTimeMap;
public:
	ActivityMgr();
	~ActivityMgr();
public:
	const std::vector<DayActivityData>& getData();
	static ActivityMgr* getInstance();
	void queryState();
	bool isThereActStart();//判断当前时间是否有某个日常活动开始啦
	inline void SetStartActIndex(int index) {_startActIndex = index;}
	inline int GetStartActIndex() {return _startActIndex;}
	void initSbkTimeMap();
	
	float GetThereActLastTime();	
	void SetTransmitFlag(bool bFlag);
	void ConfirmOneKeyTransmit();
	void ConfirmSbkKeyTransimit();
	void OneKeyTrans();//活动一键传送
	void activityTrans(int index);//活动传送
	void sbkKeyTransfer();
	void OneKeyTransForDayActivity();//每日活动传送
	DayActivityData *getDataById(int);
	bool isThereActStartForDayActivity();//只要在活动时间范围内就可以传送： 每日活动传送
	inline void SetStartActIndexForDayActivity(int index) {_startActIndexDayActivity = index;}
	inline int GetStartActIndexForDayActivity() {return _startActIndexDayActivity;}
	inline void SetStartLevelForDayActivity(int level) {_startActLevelDayActivity = level;};
	inline int GetStartLevelForDayActivity(){return _startActLevelDayActivity;};

	void ConfirmOneKeyTransmitForDayActivity();
	bool GetCurrentSBKActivityState();
	//判断是否是攻城开始当天
	bool isRemoveFirstAttackCityLimit();
	//判断沙巴克皇宫是否关闭
	bool isSbkOverTime();
	//判断是否在攻城战时间段，不判断天数
	bool isSbkStartTime();
	
	bool can_enter_sbk_result();
	sbkTimePro* GetSBKTime(enumSBKTimeType ntype);
	bool is_between_two_Index(int index1,int index2);

	bool isLuanDouHuiPrepareState();

	bool isMoshenActivityState();
	DWORD getLastMoshenTime();
	bool canJoinMoshenActivity();
	void  updateLastMoshenTime();
	bool isHasCanRecieveReward();

	bool isTianLaoTime();
	int getActEndTimes(int actIndex, bool isHourTime);

	bool getActivityDataById(int pId,DayActivityData& pData);
	
};