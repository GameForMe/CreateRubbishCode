#include "ActivityMgr.h"
#include "DBMgr.h"
#include "ServerTime.h"
#include "activeProto.h"
#include "TCPClient.h"
#include "MessageBoxUi.h"
#include "World.h"
#include "RoleManager.h"
#include "LocalPlayer.h"
#include "StringMgr.h"
#include "ToolTip.h"
#include "tagDWORDTime.h"
#include "PlayerState.h"
#include "ServerTime.h"
#include "GuildMgr.h"
#include "BloodFightMgr.h"
#include "Auto_ActionMgr.h"
#include "AnswerManager.h"
#include "f_util.h"

ActivityMgr::ActivityMgr():_startActIndex(0),_bTransmit(false),_isTransmitSBK(false),_lastSbkTime(0)
	, _startActLevelDayActivity(0), _startActIndexDayActivity(0)
{
	init_act_data();

	_lastMoshenTime = ServerTime::getInstance()->cal_current_server_dword_time() - MOSHEN_INTERVAL_TIME;

	initSbkTimeMap();
}

ActivityMgr::~ActivityMgr()
{

}


ActivityMgr* ActivityMgr::getInstance()
{
	static ActivityMgr mgr;
	return &mgr; 
}

void ActivityMgr::initSbkTimeMap()
{
// 	_sbkTimeMap.clear();
// 	c_sql_command* cmd = c_db_manager::getSingleton()->begin_operation("SELECT * FROM sbkTimeData");
// 	c_sql_table* table = c_db_manager::getSingleton()->create_table();
// 	sbkTimePro dad;
// 	while(table->retrieve_row())
// 	{
// 		unsigned int idx = table->get_index("id");
// 		dad.id = table->get_integer32(idx);
// 		idx = table->get_index("week");
// 		dad.week = table->get_integer32(idx);
// 		idx = table->get_index("hour");
// 		dad.hour = table->get_integer32(idx);
// 		idx = table->get_index("min");
// 		dad.min = table->get_integer32(idx);
// 		_sbkTimeMap.insert(std::make_pair(dad.id,dad));
// 	}
// 	c_db_manager::getSingleton()->destroy_table(table);
// 	c_db_manager::getSingleton()->end_operation(cmd);

	_sbkTimeMap.clear();
	f_data_set data;
	std::list<std::string> fields;

	bool ret = data.load("Config/db/sbkTimeData.xml","id",&fields);
	CC_ASSERT(ret);

	std::for_each(fields.begin(),fields.end(),[&](std::string const& one)
	{
		sbkTimePro dad;
		dad.id = data.get_int("id",one.c_str());
		const char* pTmp = data.get_string("week",one.c_str());
		f_util::part_string(pTmp, dad.week);
		dad.hour = data.get_int("hour",one.c_str());
		dad.min = data.get_int("min",one.c_str());

		_sbkTimeMap[dad.id] = dad;
	});
	return;
}

void ActivityMgr::queryState()
{
	//通过计算从零点零分流逝多少分钟来判断活动状态，不使用CalcTimeDiff，这个较费效率
	tagDWORDTime curtime = ServerTime::getInstance()->cal_current_server_dword_time();
	int curWeek = WhichWeekday(curtime);//得到今天是星期几，周日是0，周六是六
	int curlastmin = curtime.hour*60+curtime.min;//当前流逝的分钟数
	int beginlastmin = 0;
	int endlastmin = 0;
	int nowDay = 1<<curWeek;
	for( size_t i = 0 ; i < _allData.size() ; ++i)
	{
		beginlastmin = _allData[i].begin_hour*60+_allData[i].begin_min;//活动开始时刻流逝的分钟数
		endlastmin = _allData[i].end_hour*60+_allData[i].end_min;//活动结束时刻流逝的分钟数
		if(!(_allData[i].weekday&nowDay))
		{
			_allData[i].state=DayActivityData::S_NotToday;
			continue;
		}
		if (curlastmin < beginlastmin)//未开始
		{
			_allData[i].state = DayActivityData::S_Weikaishi;
		}
		else if (curlastmin >= beginlastmin && curlastmin <= endlastmin)//进行中
		{
			if (isOpenActive(_allData[i].id))
			{
				if (_allData[i].state == DayActivityData::S_Yijieshu)
				{
					continue;
				}
			}
			_allData[i].state = DayActivityData::S_Jinxingzhong;
		}
		else if (curlastmin > endlastmin)//已结束
		{
			_allData[i].state = DayActivityData::S_Yijieshu;
		}
		else
		{
			//do nothing
		}
		if (ACT_BOSSFIGHT_INDEX == _allData[i].id)//魔王降世 星期一，二，三，四，五，六
		{
			if (EWeek_SUN == curWeek)
			{
				_allData[i].state = DayActivityData::S_NotToday;
			}
		}
		else if (ACT_FIGHTCITY_INDEX == _allData[i].id)//攻城战 星期日
		{
			// 			if (EWeek_TUES != curWeek && EWeek_FRI != curWeek)
			// 			{
			// 				_allData[i].state = DayActivityData::S_NotToday;
			// 			}
			if (!isRemoveFirstAttackCityLimit())
			{
				_allData[i].state = DayActivityData::S_NotToday;
			}
		}
		else
		{
			//do nothing
		}
	}
}
bool ActivityMgr::GetCurrentSBKActivityState()
{
	LocalPlayer* ploc = RoleManager::getInstance()->getLocalPlayer();
	if (ploc == NULL)
	{
		return false;
	}
	if (!isRemoveFirstAttackCityLimit())
	{
		return false;
	}
	tagDWORDTime curtime = ServerTime::getInstance()->cal_current_server_dword_time();
	EWeek curWeek = ServerTime::getInstance()->get_week_day(curtime);//得到今天是星期几，周日是0，周六是六
	int curlastmin = curtime.hour*60+curtime.min;//当前流逝的分钟数
	int nowDay = 1<<curWeek;
	int beginlastmin = 0;
	int endlastmin = 0;
	_isTransmitSBK = false;
	for( size_t i = 0 ; i < _allData.size() ; ++i)
	{
		if (ACT_FIGHTCITY_INDEX != _allData[i].id )//攻城战 星期日
		{
			continue;
		}
		if(!(_allData[i].weekday&nowDay))
		{
			return false;
		}
		int level = atoi(_allData[i].level.c_str());
		if (ploc->getLevel() <level)
		{
			return false;
		}
		beginlastmin = _allData[i].begin_hour*60+_allData[i].begin_min;//活动开始时刻流逝的分钟数
		endlastmin = _allData[i].end_hour*60+_allData[i].end_min;//活动结束时刻流逝的分钟数
		//endlastmin = beginlastmin + ACT_START_MINUTE;
		if (curlastmin >= beginlastmin - 10)  
		{			
			_isTransmitSBK = true;
			_lastSbkTime = beginlastmin - curlastmin;										
		}
		if (curlastmin >= beginlastmin && curlastmin <= endlastmin)//进行中
		{
			_lastSbkTime = -1.0f;
			_isTransmitSBK = true;
		}
		if (curlastmin > endlastmin)
		{
			_lastSbkTime = -1.0f;
			_isTransmitSBK = false;
		}
	}	
	return _isTransmitSBK;
}
const std::vector<DayActivityData>& ActivityMgr::getData()
{
	queryState();
	return _allData;
}

//若返回值是空则代表当前没有开始的活动，否则返回活动名称
bool ActivityMgr::isThereActStart()
{
	LocalPlayer* player = RoleManager::getInstance()->getLocalPlayer();
	if (player == NULL)
		return false;
	tagDWORDTime curtime = ServerTime::getInstance()->cal_current_server_dword_time();
	EWeek curWeek = ServerTime::getInstance()->get_week_day(curtime);//得到今天是星期几，周日是0，周六是六
	int curlastmin = curtime.hour*60+curtime.min;//当前流逝的分钟数
	int beginlastmin = 0;
	int endlastmin = 0;
	int todayWeek = (int)1 << curWeek;
	int curlastsec = curtime.hour*60*60 + curtime.min*60 + curtime.sec;
	for( size_t i = 0 ; i < _allData.size() ; ++i)
	{
		int level = atoi(_allData[i].level.c_str());
		if (player->getLevel() < level)
		{
			continue;
		}
		if (_allData[i].id == ACT_FIGHTCITY_INDEX)
		{
			if (!isRemoveFirstAttackCityLimit())
			{
				continue;
			}
		}
		
		if(_allData[i].id == ACT_FMB_INDEX && (!RoleManager::getInstance()->getLocalPlayer()->get_fmb_active_open()))
		{
			continue;
		}

		if(_allData[i].id == ACT_BF_INDEX && (!BloodFightMgr::getInstance()->get_bf_active_open()))
		{
			continue;
		}

		if (_allData[i].id == ACT_ANSWER_INDEX && _allData[i].state == DayActivityData::S_Yijieshu)
		{
			continue;
		}

		beginlastmin = _allData[i].begin_hour*60+_allData[i].begin_min;//活动开始时刻流逝的分钟数
		endlastmin = _allData[i].end_hour*60+_allData[i].end_min;//活动结束时刻流逝的分钟数
		if (curlastmin >= beginlastmin && curlastmin <= endlastmin)//进行中
		{  
			//int timeDif = ACT_START_MINUTE_FAST;
			/*守卫白日门
			石墓夺宝
			地下夺宝
			攻城战
			魔族入侵
			海天盛宴 活动判断时间为10分钟*/
			if( _allData[i].begin_hour == 19){
				int currentTime = (beginlastmin*60 + 5*60 - curlastsec) ;
				if(currentTime > 0 && currentTime < 300){
					int minute = (currentTime)/60.0;
					int seconds = (currentTime)%60;
					std::stringstream ss;
					if(seconds <10){
						ss<<minute<<":0"<<seconds;
					}else{
						ss<<minute<<":"<<seconds;
					}
					World::getInstance()->setLeftlabel(ss.str(),currentTime);
				}else{
					World::getInstance()->setLeftlabel("",currentTime);
				}
			}

			if (_allData[i].weekday & todayWeek)
			{
				if (_bTransmit == false)
				{
					SetStartActIndex(_allData[i].id);
				}
				return true;
			}

		}
	}
	_bTransmit = false;
	SetStartActIndex(0);
	return false;
}

bool ActivityMgr::getActivityDataById(int pId,DayActivityData& pData)
{
	for( size_t i = 0 ; i < _allData.size() ; ++i)
	{
		if (_allData[i].id == pId)
		{
			pData = _allData[i];
			return true;
		}
	}
	return false;
}

float ActivityMgr::GetThereActLastTime()
{
	return _lastSbkTime;
}
void ActivityMgr::OneKeyTrans()
{
	int index = GetStartActIndex();
	DayActivityData* openItem = getDataById(index);
	if (openItem == NULL)
	{
		return;
	}
	//判断玩家当前等级是否符合
	std::string levelStr = openItem->level.substr(0,2);
	const char* str = levelStr.c_str();
	int level = (str[0] - 48) * 10 + (str[1] - 48);
	if(RoleManager::getInstance()->getLocalPlayer()->getLevel()<level){
		ToolTip::getInstance()->push(GET_STR(2392).c_str());
		return;
	}
	if (openItem)
	{
		if (openItem->id == ACT_BOSSFIGHT_INDEX)
		{
			if (!canJoinMoshenActivity())
			{
				ToolTip::getInstance()->push(GET_STR(9222));
				return;
			}
		}
		std::ostringstream oss;
		oss << openItem->name << "\xE6\xB4\xBB\xE5\x8A\xA8\xE5\xB7\xB2\xE5\xBC\x80\xE5\xA7\x8B\xEF\xBC\x8C\xE6\x82\xA8\xE6\x98\xAF\xE5\x90\xA6\xE5\x89\x8D\xE5\xBE\x80\xE5\x8F\x82\xE5\x8A\xA0";
		MessageBoxUi *transUi = MessageBoxUi::createWithTwoBtn(TEXT_UTF8_TISHI,oss.str().c_str(), TEXT_UTF8_QUEDING,TEXT_UTF8_QUXIAO);
		World::getInstance()->getScene()->addChild(transUi,WZ_MESSAGEBOX);
		transUi->signalOkBtnPressed.connect(this, &ActivityMgr::ConfirmOneKeyTransmit);
	}
	//if (GetStartActIndex() <= 0)
	//	return;
	//
	//if (index <= 0 || index > _allData.size())
	//	return;
}

void ActivityMgr::activityTrans(int index)
{
	DayActivityData* openItem = getDataById(index);
	//判断玩家当前等级是否符合
	std::string levelStr = openItem->level.substr(0,2);
	const char* str = levelStr.c_str();
	int level = (str[0] - 48) * 10 + (str[1] - 48);
	if(RoleManager::getInstance()->getLocalPlayer()->getLevel()<level){
		ToolTip::getInstance()->push(GET_STR(2392).c_str());
		return;
	}
	if (openItem)
	{
		std::ostringstream oss;
		oss << openItem->name << "\xE6\xB4\xBB\xE5\x8A\xA8\xE5\xB7\xB2\xE5\xBC\x80\xE5\xA7\x8B\xEF\xBC\x8C\xE6\x82\xA8\xE6\x98\xAF\xE5\x90\xA6\xE5\x89\x8D\xE5\xBE\x80\xE5\x8F\x82\xE5\x8A\xA0";
		MessageBoxUi *transUi = MessageBoxUi::createWithTwoBtn(TEXT_UTF8_TISHI,oss.str().c_str(), TEXT_UTF8_QUEDING,TEXT_UTF8_QUXIAO);
		World::getInstance()->getScene()->addChild(transUi,WZ_MESSAGEBOX);
		transUi->signalOkBtnPressed.connect(this, &ActivityMgr::ConfirmOneKeyTransmit);
	}
}

void ActivityMgr::sbkKeyTransfer()
{
	std::ostringstream oss;
	oss << "\xE6\xB2\x99\xE5\xB7\xB4\xE5\x85\x8B\xE5\x9F\x8E\xE6\x88\x98\xE4\xB8\x80\xE8\xA7\xA6\xE5\x8D\xB3\xE5\x8F\x91\xEF\xBC\x8C\xE6\x98\xAF\xE5\x90\xA6\xE5\x89\x8D\xE5\xBE\x80\xE5\xA4\x87\xE6\x88\x98\xEF\xBC\x9F";
	MessageBoxUi *transUi = MessageBoxUi::createWithTwoBtn(TEXT_UTF8_TISHI,oss.str().c_str(), TEXT_UTF8_QUEDING,TEXT_UTF8_QUXIAO);
	World::getInstance()->getScene()->addChild(transUi,WZ_MESSAGEBOX);
	transUi->signalOkBtnPressed.connect(this, &ActivityMgr::ConfirmSbkKeyTransimit);
}

void ActivityMgr::ConfirmOneKeyTransmit()
{
	if (GetStartActIndex() <= 0)
		return;

	NET_SIC_daily_act_transmit send;
	send.nIndex = GetStartActIndex();
	TCP_CLIENT->send_net_cmd(&send , NP_NORMAL , false);
	SetStartActIndex(0);
	_bTransmit = true;//表明已成功传送

	if (send.nIndex == ACT_ANSWER_INDEX)
	{
		AnswerManager::get_singleton_ptr()->sendOpenMessage();
		OPEN_UI(WCT_ANSWER_UI);
		return;
	}

	LocalPlayer* player = RoleManager::getInstance()->getLocalPlayer();
	player->setAttackMonsterId(0);
	if (AutoActionMgr::getInstance()->Get_Auto_Action())//gx add;
	{
		AutoActionMgr::getInstance()->Set_Auto_Action(false);
	}
	player->setAutoRunState(false);
	if (send.nIndex == ACT_BOSSFIGHT_INDEX)
	{
		updateLastMoshenTime();
	}
}

void ActivityMgr::ConfirmSbkKeyTransimit()
{
	NET_SIC_daily_act_transmit send;
	send.nIndex = 7;
	TCP_CLIENT->send_net_cmd(&send , NP_NORMAL , false);
}

bool ActivityMgr::isThereActStartForDayActivity()
{
	tagDWORDTime curtime = ServerTime::getInstance()->cal_current_server_dword_time();
	EWeek curWeek = ServerTime::getInstance()->get_week_day(curtime);//得到今天是星期几，周日是0，周六是六
	int curlastmin = curtime.hour*60+curtime.min;//当前流逝的分钟数
	int beginlastmin = 0;
	int endlastmin = 0;
	int isWhatWeek = (int)1 << curWeek;
	for( size_t i = 0 ; i < _allData.size() ; ++i)
	{
		beginlastmin = _allData[i].begin_hour*60+_allData[i].begin_min;//活动开始时刻流逝的分钟数
		endlastmin = _allData[i].end_hour*60+_allData[i].end_min;//活动结束时刻流逝的分钟数
		if (curlastmin >= beginlastmin && curlastmin <= endlastmin)//进行中
		{
 			if (_allData[i].weekday & isWhatWeek)
 			{
				SetStartActIndexForDayActivity(_allData[i].id);
				std::string levelStr = _allData[i].level.substr(0,2);
				const char* str = levelStr.c_str();
				int level = atoi(str);
				//int level = (str[0] - 48) * 10 + (str[1] - 48);
				if (level > 100)
					level = 35;
				SetStartLevelForDayActivity(level);
				return true;
			}
		}
	}
	SetStartActIndexForDayActivity(0);
	SetStartLevelForDayActivity(0);
	return false;
}



void ActivityMgr::OneKeyTransForDayActivity()
{
	unsigned int mapId = MapManager::getInstance()->getCurMapId();
	//若沙城战活动，玩家已在沙巴克，则不传送
	if (mapId == SBK_MAP_CRC_ID/* || mapId == SBK_CASTLE_MAP_CRC_ID*/)
	{
		ToolTip::getInstance()->push(GET_STR(9083).c_str());
		return;
	}
	if (GetStartActIndexForDayActivity() <= 0)
		return;
	int index = GetStartActIndexForDayActivity();
	DayActivityData* openItem = getDataById(index);
	if (openItem)
	{
		if (openItem->id == ACT_BOSSFIGHT_INDEX)
		{
			if (!canJoinMoshenActivity())
			{
				ToolTip::getInstance()->push(GET_STR(9222));
				return;
			}
		}
		std::ostringstream oss;
		oss << openItem->name << "\xE6\xB4\xBB\xE5\x8A\xA8\xE5\xB7\xB2\xE5\xBC\x80\xE5\xA7\x8B\xEF\xBC\x8C\xE6\x82\xA8\xE6\x98\xAF\xE5\x90\xA6\xE5\x89\x8D\xE5\xBE\x80\xE5\x8F\x82\xE5\x8A\xA0";
		MessageBoxUi *transUi = MessageBoxUi::createWithTwoBtn(TEXT_UTF8_TISHI,oss.str().c_str(), TEXT_UTF8_QUEDING,TEXT_UTF8_QUXIAO);
		World::getInstance()->getScene()->addChild(transUi,WZ_MESSAGEBOX);
		transUi->signalOkBtnPressed.connect(this, &ActivityMgr::ConfirmOneKeyTransmitForDayActivity);
	}
}

void ActivityMgr::ConfirmOneKeyTransmitForDayActivity()
{
	unsigned int mapId = MapManager::getInstance()->getCurMapId();
	if (mapId == SBK_MAP_CRC_ID/* || mapId == SBK_CASTLE_MAP_CRC_ID*/)
		return;
	if (GetStartActIndexForDayActivity() <= 0)
		return;

	LocalPlayer* player = RoleManager::getInstance()->getLocalPlayer();
	if (player)
	{	
		NET_SIC_daily_act_transmit send;
		send.nIndex = GetStartActIndexForDayActivity();
		TCP_CLIENT->send_net_cmd(&send ,NP_NORMAL,false);
		SetStartActIndexForDayActivity(0);
		if (send.nIndex == ACT_BOSSFIGHT_INDEX)
		{
			updateLastMoshenTime();
		}
	}
}

bool ActivityMgr::isRemoveFirstAttackCityLimit()
{
	//第二周才可以攻沙，周一为开始
	tagDWORDTime currentServerTime = ServerTime::getInstance()->cal_current_server_dword_time();
	//判断今天是周几,
	int week = WhichWeekday(currentServerTime);
	//开服天数
	int serverActivityDays = World::getInstance()->getServerActivityDay();
	int tempWeenk = week;
	if (tempWeenk == 0)
	{
		tempWeenk = 7;
	}
	if (serverActivityDays > tempWeenk)
	{
		return true;
	}
	return false;
}
DayActivityData * ActivityMgr::getDataById(int id)
{
	for (size_t i = 0;i < _allData.size();++i)
	{
		if (id == _allData.at(i).id)
		{
			return &_allData.at(i);
		}
	}

	return NULL;
}

bool ActivityMgr::isLuanDouHuiPrepareState()
{
	LocalPlayer* player = RoleManager::getInstance()->getLocalPlayer();
	if (player == NULL)
		return false;
	tagDWORDTime curtime = ServerTime::getInstance()->cal_current_server_dword_time();
	EWeek curWeek = ServerTime::getInstance()->get_week_day(curtime);//得到今天是星期几，周日是0，周六是六
	for (std::vector<DayActivityData>::iterator iter = _allData.begin(); iter != _allData.end(); ++iter)
	{
		DayActivityData info = *iter;
		if (info.id != ACT_LUANDOU_INDEX)
			continue;
		int level = atoi(info.level.c_str());
		if (player->getLevel() < level)
		{
			return false;
		}
		int todayWeek = (int)1 << curWeek;
		if ((info.weekday & todayWeek) == 0)
			continue;
		if(curtime.hour == info.begin_hour || (curtime.hour + 1) == info.begin_hour)
		{
			int nowTime = curtime.hour * 60 + curtime.min;
			int activityTime = info.begin_hour * 60 + info.begin_min ;
			int preBeginTime = activityTime - 5;
			int preEndTime = activityTime - 1;
			if (nowTime >= preBeginTime && nowTime <= preEndTime)
			{
				SetStartActIndex(ACT_LUANDOU_INDEX);
				SetStartActIndexForDayActivity(ACT_LUANDOU_INDEX);
				return true;
			}
		}
	}
	return false;
}

bool ActivityMgr::isMoshenActivityState()
{
	LocalPlayer* player = RoleManager::getInstance()->getLocalPlayer();
	if (player == NULL)
		return false;
	tagDWORDTime curtime = ServerTime::getInstance()->cal_current_server_dword_time();
	EWeek curWeek = ServerTime::getInstance()->get_week_day(curtime);//得到今天是星期几，周日是0，周六是六
	int curlastmin = curtime.hour*60+curtime.min;//当前流逝的分钟数
	for (std::vector<DayActivityData>::iterator iter = _allData.begin(); iter != _allData.end(); ++iter)
	{
		DayActivityData info = *iter;
		if (info.id != ACT_BOSSFIGHT_INDEX)
			continue;
		int level = atoi(info.level.c_str());
		if (player->getLevel() < level)
		{
			return false;
		}
		int todayWeek = (int)1 << curWeek;
		if ((info.weekday & todayWeek) == 0)
			continue;
		int beginlastmin = info.begin_hour*60+info.begin_min;//活动开始时刻流逝的分钟数
		int endlastmin = info.end_hour*60+info.end_min;//活动结束时刻流逝的分钟数
		if (curlastmin >= beginlastmin && curlastmin <= endlastmin)//进行中
		{
			//并且未完成
			if (info.comeleteNumbercishu == 0)
			{
				SetStartActIndex(info.id);
				return true;
			}
		}
	}
	return false;
}

bool ActivityMgr::canJoinMoshenActivity()
{
	DWORD nowTime = ServerTime::getInstance()->cal_current_server_dword_time();
	DWORD time = CalcTimeDiff(nowTime,_lastMoshenTime);
	if (time >= MOSHEN_INTERVAL_TIME)
	{
		return true;
	}
	return false;
}

DWORD ActivityMgr::getLastMoshenTime()
{
	return _lastMoshenTime;
}

void ActivityMgr::updateLastMoshenTime()
{
	_lastMoshenTime = ServerTime::getInstance()->cal_current_server_dword_time();
}

void ActivityMgr::SetTransmitFlag( bool bFlag )
{
	_bTransmit = bFlag;
}

bool ActivityMgr::isHasCanRecieveReward()
{
	LocalPlayer *lp = RoleManager::getInstance()->getLocalPlayer();
	if (!lp)
		return false;

	const tagActiveData &activityData = lp->getActiveData();
	const std::vector<tagHuoYueReward>&huoYueReward = Attres::getInstance()->getHuoYueRewardList();
	std::vector<tagHuoYueReward>::const_iterator iter= huoYueReward.begin();
	int index = 0;
	while(iter != huoYueReward.end())
	{
		if((iter->HuoYueReward <= activityData.m_n32_active_num) && activityData.m_b_active_receive[index] == 0)
		{
			return true;
		}
		++index;
		++iter;
	}
	return false;
}


bool ActivityMgr::isSbkOverTime()
{
	//10、沙巴克皇宫进入时间（结束）
	//6、沙巴克攻城（结束）	
	return is_between_two_Index(ESTT_10,ESTT_6);
}

bool ActivityMgr::isSbkStartTime()
{
	//5、决定参战行会（结束）								沙巴克攻城（开始）
	//6、沙巴克攻城（结束）
	return is_between_two_Index(ESTT_5,ESTT_6);
}
sbkTimePro* ActivityMgr:: GetSBKTime(enumSBKTimeType ntype)
{
	std::map<BYTE,sbkTimePro>::iterator iter = _sbkTimeMap.find(ntype);
	if (iter == _sbkTimeMap.end())
	{
		return NULL;
	}
	else
	{
		return &(iter->second);
	}
}
bool ActivityMgr::is_between_two_Index( int index1,int index2 )
{
	sbkTimePro* sbkTime1 = GetSBKTime((enumSBKTimeType)index1);
	sbkTimePro* sbkTime2 = GetSBKTime((enumSBKTimeType)index2);
	if(!sbkTime1 || !sbkTime2
		|| sbkTime1->week.size()!=sbkTime2->week.size())
	{
		return false;
	}

	sbkTimePro startTime = *sbkTime1;
	sbkTimePro endTime = *sbkTime2;
	tagDWORDTime curTime = ServerTime::getInstance()->cal_current_server_dword_time();
	for(INT i=0; i<startTime.week.size(); ++i)
	{
		if(is_between_two_time(curTime,startTime.week[i],startTime.hour,startTime.min,endTime.week[i],endTime.hour,endTime.min))
		{
			return true;
		}
	}
	
	return false;
}

bool ActivityMgr::can_enter_sbk_result()
{
	//该周能否攻城
	if (isRemoveFirstAttackCityLimit() == false)
	{
		return true;
	}
	//判断是否在开始踢人到沙巴克攻城结束之间
	//周天20:00--21:00，清除沙巴克行会，踢出非参战行会玩家
	//7、沙巴克攻城前准备，踢人开始时间
	//6、沙巴克攻城（结束）
	if (is_between_two_Index(ESTT_7,ESTT_6))
	{
		//是否有行会
		if (GuildMgr::getInstance()->getGuildId() == INVALID)
		{
			ToolTip::getInstance()->push(GET_STR(9234));
			return false;
		}
		//不是攻城行会
		if (!GuildMgr::getInstance()->isGuildAttack())
		{
			ToolTip::getInstance()->push(GET_STR(9233));
			return false;
		}
	}
	return true;
}

bool ActivityMgr::isTianLaoTime()
{
	LocalPlayer* player = RoleManager::getInstance()->getLocalPlayer();
	if (player == NULL)
		return false;

	tagDWORDTime curtime = ServerTime::getInstance()->cal_current_server_dword_time();
	EWeek curWeek = ServerTime::getInstance()->get_week_day(curtime);//得到今天是星期几，周日是0，周六是六
	int curlastmin = curtime.hour * 60 + curtime.min;//当前流逝的分钟数
	for (std::vector<DayActivityData>::iterator iter = _allData.begin(); iter != _allData.end(); ++iter)
	{
		DayActivityData info = *iter;
		if (info.id != ACT_TIANLAO_INDEX)
			continue;
		int level = atoi(info.level.c_str());
		if (player->getLevel() < level)
		{
			return false;
		}
		int todayWeek = (int)1 << curWeek;
		if ((info.weekday & todayWeek) == 0)
			continue;

		int beginlastmin = info.begin_hour * 60 + info.begin_min;//活动开始时刻流逝的分钟数
		int endlastmin = info.end_hour * 60 + info.end_min;//活动结束时刻流逝的分钟数
		if (curlastmin >= beginlastmin && curlastmin <= endlastmin)//进行中
		{
			return true;
		}
	}
	
	return false;
}

int ActivityMgr::getActEndTimes(int actIndex, bool isHourTime)
{
	if(actIndex < 0)
		return INVALID;

	for( size_t i = 0 ; i < _allData.size() ; ++i)
	{
		if(_allData[i].id == actIndex)
		{
			int returnTime;
			if(isHourTime){
				returnTime = _allData[i].end_hour;
			}else{
				returnTime = _allData[i].end_min;
			}
			return returnTime;
		}
	}

	return INVALID;
}

void ActivityMgr::init_act_data()
{
	DayActivityData dad;

	f_data_set data_set;
	std::list<std::string> fields;
	bool ret =data_set.load("Config/db/ActivityData.xml","id",&fields);
	CC_ASSERT(ret);
	std::for_each(	fields.begin(),
					fields.end(),
					[&](std::string const& one)
	{

		dad.id = data_set.get_int("id",one.c_str());
		dad.name= data_set.get_string("activity_name",one.c_str(),"");
		dad.npcName= data_set.get_string("npc_name",one.c_str(),"");
		dad.mapID= data_set.get_int("mapid",one.c_str(),0);
		dad.reward= data_set.get_string("reward",one.c_str(),"");
		dad.time= data_set.get_string("time_str",one.c_str(),"");
		dad.desc= data_set.get_string("desc",one.c_str(),"");
		dad.level= data_set.get_string("join_lv",one.c_str(),"");
		dad.begin_hour= data_set.get_int("begin_hour",one.c_str());
		dad.begin_min= data_set.get_int("begin_min",one.c_str());
		dad.end_hour= data_set.get_int("end_hour",one.c_str());
		dad.end_min= data_set.get_int("end_min",one.c_str());

		dad.x= data_set.get_int("x",one.c_str(),0);
		dad.y= data_set.get_int("y",one.c_str(),0);
		dad.item[0]= data_set.get_int("item1",one.c_str(),0);
		dad.item[1]= data_set.get_int("item2",one.c_str(),0);
		dad.item[2]= data_set.get_int("item3",one.c_str(),0);
		
		dad.comeleteNumbercishu= data_set.get_int("comeleteNumbercishu",one.c_str(),0);
		dad.huoyuedu = data_set.get_int("huoyuedu",one.c_str(),0);
		dad.weekday = data_set.get_int("weekday",one.c_str(),0);
		dad.openLv = data_set.get_int("openLv",one.c_str(),0);

		dad.transfer_pic= data_set.get_string("Transfer_Pic",one.c_str(),"");
		dad.pic_eff= data_set.get_string("Pic_Effect",one.c_str(),"");

		dad.type = data_set.get_int("Type",one.c_str(),0);
		dad.openUI = data_set.get_int("open_ui",one.c_str(),0);
		dad.is_team_show = data_set.get_int("sign",one.c_str(),0);

		_allData.push_back(dad);
	});

	/*
	c_sql_command* cmd = c_db_manager::getSingleton()->begin_operation("SELECT * FROM ActivityInfo");
	c_sql_table* table = c_db_manager::getSingleton()->create_table();
	DayActivityData dad;
	//int index = 0;
	while(table->retrieve_row())
	{
		unsigned int idx = table->get_index("activity_name");
		dad.name = table->get_text(idx);
		idx = table->get_index("npc_name");
		dad.npcName = table->get_text(idx);
		idx = table->get_index("mapid");
		dad.mapID = table->get_integer32(idx);
		idx = table->get_index("reward");
		dad.reward = table->get_text(idx);
		idx = table->get_index("time_str");
		dad.time = table->get_text(idx);
		idx = table->get_index("desc");
		dad.desc = table->get_text(idx);
		idx= table->get_index("join_lv");
		dad.level=table->get_text(idx);
		idx = table->get_index("begin_hour");
		dad.begin_hour = table->get_integer32(idx);
		idx = table->get_index("begin_min");
		dad.begin_min = table->get_integer32(idx);
		idx = table->get_index("end_hour");
		dad.end_hour = table->get_integer32(idx);
		idx = table->get_index("end_min");
		dad.end_min = table->get_integer32(idx);
		idx = table->get_index("x");
		dad.x = table->get_integer32(idx);
		idx = table->get_index("y");
		dad.y = table->get_integer32(idx);
		idx=table->get_index("item1");
		dad.item[0]=table->get_integer32(idx);
		idx=table->get_index("item2");
		dad.item[1]=table->get_integer32(idx);
		idx=table->get_index("item3");
		dad.item[2]=table->get_integer32(idx);
		idx=table->get_index("comeleteNumbercishu");
		dad.comeleteNumbercishu=table->get_integer32(idx);
		idx=table->get_index("huoyuedu");
		dad.huoyuedu=table->get_integer32(idx);
		idx=table->get_index("weekday");
		dad.weekday=table->get_integer32(idx);
		idx=table->get_index("id");
		dad.id=table->get_integer32(idx);
		idx = table->get_index("openLv");
		dad.openLv = table->get_integer32(idx);
		idx = table->get_index("Transfer_Pic");
		dad.transfer_pic = table->get_text(idx);
		idx = table->get_index("Pic_Effect");
		dad.pic_eff = table->get_text(idx);
		idx = table->get_index("Type");
		dad.type = table->get_integer32(idx);
		//dad.actid = ++index;//gx add
		idx = table->get_index("open_ui");
		dad.openUI = table->get_integer32(idx);
		idx = table->get_index("sign");
		dad.is_team_show = table->get_integer32(idx);
		_allData.push_back(dad);
	}
	c_db_manager::getSingleton()->destroy_table(table);
	c_db_manager::getSingleton()->end_operation(cmd);
	*/
}

