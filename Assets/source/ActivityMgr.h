#pragma once
#include <string>
#include <vector>
#include"Attres.h"
#define ACT_START_MINUTE_FAST	2 //ÿ�ջ��ʼ��ǰ��������UI
#define ACT_START_MINUTE_SLOW	10//ÿ�ջ��ʼ��ǰʮ������UI
//��������
#define ACT_LUANDOU_INDEX		2 //�Ҷ���
#define ACT_BOSSFIGHT_INDEX		8 //ħ������
#define ACT_FIGHTCITY_INDEX		7 //����ս
#define ACT_TIANLAO_INDEX       12//�´�����
#define ACT_FMB_INDEX			14
#define ACT_BF_INDEX			15
#define ACT_ANSWER_INDEX        16//����
#define isOpenActive(num)       ((num) == ACT_ANSWER_INDEX)

#define MOSHEN_INTERVAL_TIME	10//ħ�������μӵļ��ʱ��
struct DayActivityData
{
	enum eState
	{
		S_Jinxingzhong,
		S_Weikaishi,
		S_Yijieshu,
		S_NotToday,//gx add 2014.2.17����ս���л�BOSSս����
		S_Finish  //�����
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
	//int actid;//gx add 2013.12.23 ��¼�ID
	int item[JiangLiNumber];
	int comeleteNumbercishu;//�ܴ���
	int huoyuedu;
	int weekday;//�ܼ���ʼ
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


//ɳ��սʱ���
//����																		
//1�������л��ʽ𣬾����ʽ����㣬ȡ����ս״̬����ʼ��
//2�������л��ʽ𣬾����ʽ����㣬ȡ����ս״̬��������   ���꣨��ʼ��		
//3�����꣨������															
//4��������ս�лᣨ��ʼ��													
//5��������ս�лᣨ������								ɳ�Ϳ˹��ǣ���ʼ��
//6��ɳ�Ϳ˹��ǣ�������														
//7��ɳ�Ϳ˹���ǰ׼�������˿�ʼʱ��
//8��ɳ�Ϳ˹���ǰ׼�������˽���ʱ��
//9��ɳ�Ϳ˻ʹ�����ʱ�䣨��ʼ��
//10��ɳ�Ϳ˻ʹ�����ʱ�䣨������

//����������
//1��
//2�������л��ʽ𣬾����ʽ����㣬ȡ����ս״̬��������		�������л��ʽ𣬾����ʽ����㣬ȡ����ս״̬����ʼ��
//3��
//4���������л��ʽ𣬾����ʽ����㣬ȡ����ս״̬��������	�������л��ʽ𣬾����ʽ����㣬��ȡ����ս״̬����������ս����ʼ��
//5��
//6�������л��ʽ𣬾����ʽ����㣬ȡ����ս״̬����ʼ��		�������л��ʽ𣬾����ʽ����㣬��ȡ����ս״̬����������ս��������

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
	bool _bTransmit;//�Ƿ��� 
	bool _isTransmitSBK; 
	float _lastSbkTime;
	DWORD _lastMoshenTime;//�ϴε��ħ��������ʱ��

	void init_act_data();
	//ɳ��սʱ���
	//����																		
	//1�������л��ʽ𣬾����ʽ����㣬ȡ����ս״̬����ʼ��
	//2�������л��ʽ𣬾����ʽ����㣬ȡ����ս״̬��������   ���꣨��ʼ��		
	//3�����꣨������															
	//4��������ս�лᣨ��ʼ��													
	//5��������ս�лᣨ������								ɳ�Ϳ˹��ǣ���ʼ��
	//6��ɳ�Ϳ˹��ǣ�������														
	//7��ɳ�Ϳ˹���ǰ׼�������˿�ʼʱ��
	//8��ɳ�Ϳ˹���ǰ׼�������˽���ʱ��

	//����������
	//1��
	//2�������л��ʽ𣬾����ʽ����㣬ȡ����ս״̬��������		�������л��ʽ𣬾����ʽ����㣬ȡ����ս״̬����ʼ��
	//3��
	//4���������л��ʽ𣬾����ʽ����㣬ȡ����ս״̬��������	�������л��ʽ𣬾����ʽ����㣬��ȡ����ս״̬����������ս����ʼ��
	//5��
	//6�������л��ʽ𣬾����ʽ����㣬ȡ����ս״̬����ʼ��		�������л��ʽ𣬾����ʽ����㣬��ȡ����ս״̬����������ս��������
	std::map<BYTE,sbkTimePro>	_sbkTimeMap;
public:
	ActivityMgr();
	~ActivityMgr();
public:
	const std::vector<DayActivityData>& getData();
	static ActivityMgr* getInstance();
	void queryState();
	bool isThereActStart();//�жϵ�ǰʱ���Ƿ���ĳ���ճ����ʼ��
	inline void SetStartActIndex(int index) {_startActIndex = index;}
	inline int GetStartActIndex() {return _startActIndex;}
	void initSbkTimeMap();
	
	float GetThereActLastTime();	
	void SetTransmitFlag(bool bFlag);
	void ConfirmOneKeyTransmit();
	void ConfirmSbkKeyTransimit();
	void OneKeyTrans();//�һ������
	void activityTrans(int index);//�����
	void sbkKeyTransfer();
	void OneKeyTransForDayActivity();//ÿ�ջ����
	DayActivityData *getDataById(int);
	bool isThereActStartForDayActivity();//ֻҪ�ڻʱ�䷶Χ�ھͿ��Դ��ͣ� ÿ�ջ����
	inline void SetStartActIndexForDayActivity(int index) {_startActIndexDayActivity = index;}
	inline int GetStartActIndexForDayActivity() {return _startActIndexDayActivity;}
	inline void SetStartLevelForDayActivity(int level) {_startActLevelDayActivity = level;};
	inline int GetStartLevelForDayActivity(){return _startActLevelDayActivity;};

	void ConfirmOneKeyTransmitForDayActivity();
	bool GetCurrentSBKActivityState();
	//�ж��Ƿ��ǹ��ǿ�ʼ����
	bool isRemoveFirstAttackCityLimit();
	//�ж�ɳ�Ϳ˻ʹ��Ƿ�ر�
	bool isSbkOverTime();
	//�ж��Ƿ��ڹ���սʱ��Σ����ж�����
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