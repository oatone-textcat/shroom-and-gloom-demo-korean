using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace KoreanKeywordTooltipPatch
{
    [BepInPlugin("com.oatone.shroomandgloom.koreankeywordtooltip", "Korean Keyword Tooltip Patch", "1.0.0")]
    public class Plugin : BasePlugin
    {
        internal static ManualLogSource Logger;

        public override void Load()
        {
            Logger = Log;

            Log.LogInfo("Korean Keyword Tooltip Patch loaded.");

            TextWithLinksState.LogFieldStatus();

            var harmony = new Harmony("com.oatone.shroomandgloom.koreankeywordtooltip");

            harmony.PatchAll(typeof(TextWithLinksLinkLocalizePatch));
            harmony.PatchAll(typeof(TMPLinkTextAliasPatch));
            harmony.PatchAll(typeof(TextWithLinksDeselectSafetyPatch));
            harmony.PatchAll(typeof(TextWithLinksShowHoverOverlaySafetyPatch));
            harmony.PatchAll(typeof(TextWithLinksLateUpdateSafetyPatch));
            //harmony.PatchAll(typeof(TMPTextSetTextPatch));


            Log.LogInfo("Harmony patches applied.");
        }
    }
    internal static class ExceptionUtil
    {
        public static bool IsIndexOutOfRange(Exception e)
        {
            if (e == null)
                return false;

            if (e is IndexOutOfRangeException)
                return true;

            string text = e.ToString();
            return text.Contains("IndexOutOfRangeException") ||
                   text.Contains("Index was outside the bounds of the array");
        }
    }

    internal static class KeywordAlias
    {
        private static readonly Dictionary<string, string> Map = new()
        {

            ["생명체"] = "Creature",
            ["음식"] = "Food",
            ["스킬"] = "Skill",
            ["없음"] = "None",
            ["패시브"] = "Passive",
            ["공격"] = "Attack",
            ["기분"] = "Mood",
            ["저주"] = "Curse",
            ["기계"] = "Machine",
            ["충전"] = "Charge",
            ["데이터"] = "Data",
            ["캠프"] = "Camp",
            ["열쇠"] = "Pick",
            ["탐색"] = "Search",
            ["죽음"] = "Death",
            ["특수"] = "Special",
            ["예견"] = "Foresight",
            ["처치 시"] = "IF FATAL",
            ["오물 제거 시"] = "IF GUNK IS REMOVED on this card",
            ["제거한 오물마다"] = "FOR EVERY GUNK REMOVED",
            ["완전히 오물 제거 시"] = "IF FULLY UNGUNKED",
            ["카드에서 오물 제거 시"] = "IF UNGUNKING CARD",
            ["저장된 오물 소모 시"] = "IF STORED GUNK CONSUMED",
            ["오물 제거 시"] = "IF GUNK REMOVED on target",
            ["오물투성이 카드가 있을 시"] = "IF ANY CARD GUNKED",
            ["모든 오물 제거 시"] = "IF ALL GUNK REMOVED on any card",
            ["오물투성이일 시"] = "IF GUNKED",
            ["과충전 시"] = "WHEN OVERCHARGED",
            ["과충전 종료 시"] = "WHEN OVERCHARGE ENDS",
            ["카드가 과충전될 시"] = "WHEN ANY CARD OVERCHARGES",
            ["과충전이 종료될 시"] = "WHEN ANY OVERCHARGE ENDS",
            ["두꺼비"] = "TOAD",
            ["섭취"] = "EAT",
            ["충전"] = "CHARGE",
            ["구조물"] = "STRUCTURE",
            ["이상한"] = "ODD",
            ["저장"] = "STORE",
            ["오물투성이"] = "GUNKY",
            ["톱니바퀴"] = "GEAR",
            ["추방"] = "EXILE",
            ["시전"] = "CAST",
            ["버리기"] = "DISCARD",
            ["소멸"] = "EXHAUST",
            ["단백질을 저장"] = "STORE PROTEIN",
            ["섭식"] = "SWALLOW",
            ["방전"] = "DISCHARGE",
            ["충전됨"] = "CHARGED",
            ["소모"] = "CONSUME",
            ["무력화"] = "CRIPPLE",
            ["거미 알을 낳습니다"] = "LAY A SPIDER EGG",
            ["펫을 부릅니다"] = "CALL A PET",
            ["뚱뚱한 굼벵이"] = "FAT GRUB",
            ["트러플 돼지"] = "TRUFFLE PIG",
            ["끈적한 점액"] = "STICKY GOO",
            ["혹은"] = "OR",
            ["그리고"] = "AND",
            ["파괴 가능"] = "BREAKABLE",
            ["발굴 가능"] = "DIGGABLE",
            ["먹이주기 가능"] = "FEEDABLE",
            ["점화 가능"] = "IGNITABLE",
            ["수리 가능"] = "FIXABLE",
            ["공개 가능"] = "REVEALABLE",
            ["개방 가능"] = "UNLOCKABLE",
            ["사용할 때"] = "WHEN USED",
            ["단백질"] = "Protein",
            ["개방"] = "Unlock",
            ["발굴"] = "Dig",
            ["파괴"] = "Break",
            ["수리"] = "Fix",
            ["공개"] = "Reveal",
            ["점화"] = "Ignite",
            ["먹이주기"] = "Feed",
            ["명상"] = "Meditate",
            ["없음"] = "Nothing",
            ["흡수"] = "Absorb",
            ["어둠 증폭"] = "AMPLIFY GLOOM",
            ["아날로그"] = "Analog",
            ["아무"] = "Any",
            ["배터리"] = "Battery",
            ["캠프"] = "Camp",
            ["쿠폰"] = "Coupon",
            ["데이터"] = "Data",
            ["불사"] = "Deathless",
            ["긴장된"] = "Nervous",
            ["버섯 난"] = "Shroom",
            ["그을린"] = "Singed",
            ["이상한"] = "Weird",
            ["야성적인"] = "Wild",
            ["활력"] = "Energize",
            ["에너지 비용"] = "Energy Cost",
            ["진화"] = "Evolve",
            ["소멸"] = "Exhaust",
            ["온기"] = "Warmth",
            ["음식"] = "Food",
            ["취급주의"] = "Fragile",
            ["골드"] = "Gold",
            ["오물을 추가하여 뽑기"] = "Draw With Gunk",
            ["오물"] = "Gunk",
            ["저장된 오물"] = "Stored Gunk",
            ["목마름"] = "Thirsty",
            ["능숙"] = "Handy",
            ["공포"] = "Horror",
            ["처치 시 트리거"] = "If Fatal Trigger",
            ["불가피"] = "Inescapable",
            ["보존"] = "Linger",
            ["기분"] = "Mood",
            ["지속"] = "Persistent",
            ["격노"] = "Rage",
            ["보충"] = "Replenish",
            ["멸균"] = "Sterile",
            ["어둠"] = "Gloom",
            ["임시"] = "Temporary",
            ["사용 불가"] = "Unplayable",
            ["남은 횟수:"] = "Uses",
            ["영구"] = "Permanent",
            ["수리 가능"] = "FIXABLE",
            ["파괴 가능"] = "BREAKABLE",
            ["발굴 가능"] = "DIGGABLE",
            ["먹이주기 가능"] = "FEEDABLE",
            ["발화 가능"] = "IGNITABLE",
            ["공개 가능"] = "REVEALABLE",
            ["개방 가능"] = "UNLOCKABLE",
            ["탐험"] = "Explore",
            ["전투"] = "Combat",
            ["예견"] = "Foresight",

            ["정신증"] = "Psychosis",
            ["간이끼"] = "Liver Wort",
            ["뿔이끼"] = "Horn Wort",
            ["찌든때의 저주"] = "Crusty Curse",
            ["애정 어린"] = "Loving",
            ["충전됨"] = "Charged",
            ["전도성"] = "Conductive",
            ["진화하는"] = "Evolving",
            ["음울의 피조물"] = "Gloom Creature",
            ["배짱 있는"] = "Gutsy",
            ["화가 난"] = "Irritable",
            ["팔다리"] = "Limb",
            ["전리품"] = "Loot",
            ["변덕스러운"] = "Moody",
            ["이끼 낀"] = "Mossy",
            ["겁쟁이"] = "Scaredy",
            ["점액질"] = "Slimey",
            ["팀 플레이어"] = "Team Player",
            ["가시"] = "Thorns",
            ["비행"] = "Airborne",
            ["장갑"] = "Armoured",
            ["축복"] = "Bless",
            ["무지성"] = "Brainless",
            ["흐름 끊기"] = "Buzzkill",
            ["부착"] = "Lodged",
            ["혼란"] = "Confused",
            ["무력화"] = "Crippled",
            ["불사"] = "Deathless",
            ["부패"] = "Decay",
            ["분열 중"] = "Dividing",
            ["격노"] = "Enrage",
            ["영체 상태"] = "Ethereal",
            ["결실 맺기"] = "Fruiting",
            ["잉태 중"] = "Gestating",
            ["핫소스"] = "Hot Sauce",
            ["감염됨"] = "Infested",
            ["통찰"] = "Insight",
            ["고무됨"] = "Inspired",
            ["재생"] = "Regrowth",
            ["재기"] = "Resurgence",
            ["썩은"] = "Rotten",
            ["양념"] = "Seasoned",
            ["슬라임 엄마"] = "Slime Mom",
            ["주문 취약"] = "Spell Weakness",
            ["주문 보호막"] = "Spellshield",
            ["포자투성이의"] = "Sporey",
            ["굶주림"] = "Starving",
            ["기이한 출구"] = "Strange Exit",
            ["기절"] = "Stun",
            ["신속함"] = "Swift",
            ["가시 돋친"] = "Thorned",
            ["소심함"] = "Timid",
            ["불안감을 주는"] = "Unnerving",
            ["비열한"] = "Vile",
            ["취약함"] = "Vulnerable",

            ["찌르기"] = "Stab",
            ["2M 밧줄"] = "2M Rope",
            ["흡수"] = "Absorb",
            ["아카시아 철퇴"] = "Acacia Mace",
            ["잔광"] = "Afterglow",
            ["섭리의 눈"] = "All seeing eye",
            ["분석"] = "Analysis",
            ["암살"] = "Assasinate",
            ["전동 톱"] = "Auto Saw",
            ["복수자"] = "Avenger",
            ["끝내주는 수프"] = "Awesome Soup",
            ["등 긁기"] = "Back Scratch",
            ["배낭"] = "Backpack",
            ["상한 소고기"] = "Bad Beef",
            ["열쇠 주머니"] = "Bag Of Picks",
            ["비밀 주머니"] = "Bag of Secrets",
            ["붕대"] = "Bandages",
            ["반창고"] = "Bandaid",
            ["세례"] = "Baptise",
            ["가시 돋친 덩굴"] = "Barbed Vine",
            ["토사물 열쇠"] = "Barf Pick",
            ["강타"] = "Bash",
            ["박쥐 열쇠"] = "Bat Pack",
            ["대형 손전등"] = "Beamer",
            ["시원한 한 잔"] = "Big Gulp",
            ["담즙 발사"] = "Bile Blast",
            ["생체 제련소"] = "Bio Forge",
            ["바이오연료 발전기"] = "Biofuel Generator",
            ["물어뜯기"] = "Bite",
            ["입에 쓴 약"] = "Bitter Medicine",
            ["기묘한 토스티"] = "Bizzare Toasty",
            ["날카로운 탈출"] = "Bladed Escape",
            ["축복의 토스티"] = "Blessed Toasty",
            ["블릿츠"] = "Blitz",
            ["방어 망치"] = "Blocking Hammer",
            ["피바다"] = "Blood Bath",
            ["토치"] = "Blowtorch",
            ["파란색 수정"] = "Blue Crystal",
            ["볼트"] = "Bolts",
            ["뼈 톱"] = "Bone Saw",
            ["뼈 가시"] = "Bone Spur",
            ["뼈다귀 열쇠"] = "Bone To Pick",
            ["도넛 상자"] = "Box of Donuts",
            ["성냥 상자"] = "Box of Matches",
            ["팅키 상자"] = "Box of Tinkies",
            ["빵가루"] = "Bread Crumbs",
            ["가시덤불 망치"] = "Briar Hammer",
            ["근육질 손가락"] = "Buff Fingers",
            ["벌레 공격"] = "Bug Attack",
            ["분젠 버너"] = "Bunsen Burner",
            ["불타는 분노"] = "Burning Rage",
            ["불타는 와인"] = "Burning Wine",
            ["터지는 벌레"] = "Bursting Butts",
            ["계산기"] = "Calculator",
            ["피의 부름"] = "Call for Blood",
            ["캠프파이어"] = "Camp Fire",
            ["카나리아"] = "Canary",
            ["신중한 계획"] = "Careful Planning",
            ["당근"] = "Carrot",
            ["카타르시스"] = "Catharsis",
            ["건전지"] = "Cell",
            ["까맣게 탄 토스티"] = "Charred Toasty",
            ["싸구려 라이터"] = "Cheap Lighter",
            ["씹던 껌"] = "Chewed Gum",
            ["연골 씹기"] = "Chewing Gristle",
            ["휘젓는 터렛"] = "Churning Turret",
            ["청소 도구"] = "Cleaning Supplies",
            ["맑은 정신"] = "Clear Head",
            ["영혼의 단짝"] = "Close Friend",
            ["고물"] = "Clunker",
            ["커피 스테이션"] = "Coffee Station",
            ["관짝"] = "Coffin",
            ["지휘관"] = "Commander",
            ["끝없는 분노"] = "Constant Rage",
            ["타락한 팔다리"] = "Corrupted Limb",
            ["우주적 공포"] = "Cosmic Horror",
            ["쿠폰"] = "Coupon",
            ["생명체"] = "Creature",
            ["무력화 타격"] = "Crippling Blow",
            ["찌꺼기 쿠키"] = "Crud Cookie",
            ["찌꺼기 터렛"] = "Crud Turret",
            ["때에 찌든 칼날"] = "Crusted Blade",
            ["찌든때의 저주"] = "Crusty Curse",
            ["수정구"] = "Crystal Ball",
            ["핫초코 한 잔"] = "Cup of Cocoa",
            ["커피 한 잔"] = "Cup of Coffee",
            ["호기심많은 벌레"] = "Curious Bug",
            ["식기 세트"] = "Cutlery",
            ["어둠의 조력자"] = "Dark Acquaintance",
            ["어둠의 수정"] = "Dark Crystal",
            ["데이터"] = "Data",
            ["치명적인 곰팡이"] = "Deadly Mould",
            ["죽움의 뚜러뻥"] = "Death Plunger",
            ["죽음의 시선"] = "Death Stare",
            ["찌그러진 보온병"] = "Dented Thermous",
            ["절박함"] = "Desperation",
            ["파괴자"] = "Destroyer",
            ["탐정"] = "Detective",
            ["주사위"] = "Dice",
            ["영면"] = "Dirt Nap",
            ["더러운 가시"] = "Dirty Splinter",
            ["규율"] = "Discipline",
            ["해부대"] = "Dissection Table",
            ["소화 주스"] = "Dissolving Juices",
            ["아득한 그림자"] = "Distant Shadow",
            ["DJ 붐바"] = "DJ Boomba",
            ["도넛"] = "Donut",
            ["공포"] = "Dread",
            ["드릴"] = "Drill",
            ["메아리치는 피리"] = "Echoing Flute",
            ["전기 꼬챙이"] = "Electro Prod",
            ["감정의 롤러코스터"] = "Emotional Rollercoaster",
            ["직무 참고서"] = "Employee Handbook",
            ["빈 항아리"] = "Empty Jar",
            ["격려"] = "Encouragement",
            ["끝없는 유충"] = "Endless Larvae",
            ["끝없는 수프"] = "Endless Soup",
            ["방사(능)"] = "eRADIATE",
            ["유레카!"] = "EUREKA!",
            ["영원의 트러플"] = "Everlasting Truffle",
            ["누더기 덧댐"] = "Extra Padding",
            ["눈알"] = "Eyeball",
            ["익숙함"] = "Familiarity",
            ["고통 애호가"] = "Fan Of Pain",
            ["통통한 굼벵이"] = "Fat Grub",
            ["피로"] = "Fatigue",
            ["손재주"] = "Feeling Handy",
            ["야성적인 돌격"] = "Feral Charge",
            ["풍요로운 성장"] = "Fertile Growth",
            ["썩어가는 더미"] = "Festering Mound",
            ["고열"] = "Fever",
            ["불타는 수프"] = "Fiery Soup",
            ["더러운 습관"] = "Filthy Habit",
            ["폭죽"] = "Firecrackers",
            ["정비공"] = "Fixer",
            ["흐릿한 검"] = "Flaccid Sword",
            ["채찍질"] = "Flagellate",
            ["플람베"] = "Flambee",
            ["화염 터렛"] = "Flame Turret",
            ["불타는 포크"] = "Flaming Fork",
            ["조명탄"] = "Flare",
            ["플래시"] = "Flash",
            ["섬광 터렛"] = "Flash Turret",
            ["손전등"] = "Flashlight",
            ["부싯돌"] = "Flint",
            ["날아차기"] = "Fly kick",
            ["집중"] = "Focus",
            ["음식벌레"] = "Foodworm",
            ["채집한 간식"] = "Foraged Snack",
            ["강제 성장"] = "Forced Growth",
            ["예감"] = "Foreboding",
            ["행운"] = "Fortune",
            ["악취나는 숨결"] = "Foul Breath",
            ["썩어가는 덤불"] = "Foul Vegetation",
            ["필사적인 수색"] = "Frantic Search",
            ["친근한 불씨"] = "Friendly Ember",
            ["증식하는 팔다리"] = "Fruiting Limb",
            ["짜증"] = "Frustration",
            ["헛손질 베기"] = "Fumble slash",
            ["균류 빵"] = "Fungal Bread",
            ["균류 덩어리빵"] = "Fungal Loaf",
            ["최애 간식"] = "Go-To Snack",
            ["황금 꽃가루"] = "Golden Pollen",
            ["파내기"] = "Gouge",
            ["미식의 토스티"] = "Gourmet Toasty",
            ["거대한 곤봉"] = "Grand Maul",
            ["윤활유"] = "Grease",
            ["탐욕의 단검"] = "Greedy Dagger",
            ["초록색 수정"] = "Green Crystal",
            ["마도서"] = "Grimoire",
            ["연골 뱉기"] = "Gristle Spit",
            ["촉진의 수프"] = "Growing Soup",
            ["꼬질꼬질한 손가락"] = "Grubby Fingers",
            ["굼벵이들"] = "Grubs",
            ["원한"] = "Grudge",
            ["오물 발사기"] = "Gunk Blaster",
            ["오물 수확기"] = "Gunk Harvester",
            ["오물 허파"] = "Gunk Lung",
            ["오물 토스터"] = "Gunk Toaster",
            ["오물 콜라"] = "Gunka Cola",
            ["오포칼립스"] = "Gunkpocalypse",
            ["오물투성이 가젯팔"] = "Gunky Grabber",
            ["내장"] = "Guts",
            ["망치"] = "Hammer",
            ["방호복"] = "Hazmat Suit",
            ["든든한 수프"] = "Hearty Soup",
            ["공허한 햄스터"] = "Hollow Hamster",
            ["성스러운 굼벵이"] = "Holy Grub",
            ["성스러운 수프"] = "Holy Soup",
            ["한 줄기 희망"] = "Hope",
            ["뿔이끼"] = "Hornwart",
            ["끔찍한 몽둥이"] = "Horrible Club",
            ["달궈진 불쏘시개"] = "Hot Poker",
            ["핫소스"] = "Hot Sauce",
            ["뜨거운 토스티"] = "Hot Toasty",
            ["포옹 공격"] = "Hug Attack",
            ["굶주린 칼날"] = "Hungry blade",
            ["선택장애"] = "Indecision",
            ["감염된 팔다리"] = "Infected Limb",
            ["감염된 톱"] = "Infected Saw",
            ["불지옥"] = "Inferno",
            ["탐욕스러운 굼벵이"] = "Insatiable Grub",
            ["영감"] = "Inspiration",
            ["영감의 열쇠"] = "Inspiring Pick",
            ["단열재"] = "Insulation",
            ["발명가의 칼날"] = "Inventors blade",
            ["가려운 손가락"] = "Itchy Fingers",
            ["찌르개"] = "Jabber",
            ["정크푸드"] = "Junk Food",
            ["열쇠"] = "Key",
            ["키 카드"] = "Key Card",
            ["주방"] = "Kitchen",
            ["게으른 손가락"] = "Lazy Fingers",
            ["썩어 문드러진 것"] = "Left To Rot",
            ["남은 식량"] = "Leftovers",
            ["핥기"] = "Lick",
            ["유연한 탈출"] = "Limber Escape",
            ["남아있는 온기"] = "Lingering Warmth",
            ["자물쇠 파괴자"] = "Lock Breaker",
            ["자물쇠 포식자"] = "Lock Eater",
            ["자물쇠 도리깨"] = "Lock Flail",
            ["락피그"] = "Lock Pig",
            ["자물쇠 점액 도포기"] = "Lock Slimer",
            ["열쇠"] = "Lockpick",
            ["길 잃은 영혼"] = "Lost Soul",
            ["사랑스러운 기생충"] = "Loving parasite",
            ["충성스러운 카나리아"] = "Loyal Canary",
            ["충성그러운 새끼돼지"] = "Loyal Piglet",
            ["미끈거림"] = "Lubricated",
            ["지도"] = "Map",
            ["마시멜로"] = "Marshmallows",
            ["구급로봇"] = "Medibot",
            ["구급상자"] = "Medical Kit",
            ["명상"] = "Meditate",
            ["꾀죄죄한 화염귀"] = "Messy Blowtorch",
            ["초소형 터렛"] = "Micro Turret",
            ["미미한 성장"] = "Minor Growth",
            ["어린 두더쥐"] = "Molechild",
            ["괴물 주스"] = "Monster Juice",
            ["아침 스트레칭"] = "Morning Stretches",
            ["이끼 낀 전투식량"] = "Mossy Rations",
            ["이끼 간식"] = "Mossy Snack",
            ["의욕 충만"] = "Motivated",
            ["털갈이"] = "Moult",
            ["멀리건"] = "Muligan",
            ["중량 망치"] = "Muscle Mallet",
            ["근육 경련"] = "Muscle Spasm",
            ["버섯 열쇠"] = "Mushroom Key",
            ["돌연변이 열쇠"] = "Mutant Pick",
            ["돌연변이 강타"] = "Mutant Smash",
            ["못"] = "Nail",
            ["공책"] = "Notebook",
            ["영양 주먹"] = "Nurti Fists",
            ["영양분 합성기"] = "Nutrient Fabricator",
            ["이상한 돌"] = "Odd Rock",
            ["분비물"] = "Ooze",
            ["질척이는 용기"] = "Oozing Canister",
            ["질척이는 살덩이"] = "Oozing Flesh",
            ["격분"] = "Outburst",
            ["무리하기"] = "Over Do It",
            ["건전지 한 묶음"] = "Pack O cELLS",
            ["계약"] = "Pact",
            ["패드코스"] = "Padkos",
            ["진통제"] = "Painkillers",
            ["공황"] = "Panic",
            ["개인적 성장"] = "Personal Growth",
            ["곡괭이"] = "Pick Axe",
            ["편식쟁이"] = "Picky Eater",
            ["돼지 기생충"] = "Pig Louse",
            ["돼지우리"] = "Pig Pen",
            ["피펫"] = "Pipette",
            ["열받음"] = "Pissed Off",
            ["펜치"] = "Pliers",
            ["긍정적 마음가짐"] = "PMA",
            ["보풀 덩어리"] = "Pocket Lint",
            ["가능성의 수정"] = "Possibility Crystal",
            ["파워 낮잠"] = "Power Nap",
            ["파워 낮잠"] = "Powernap",
            ["훈련용 허수아비"] = "Practice Dummy",
            ["소중한 한입"] = "Precious Morsel",
            ["탐침"] = "Probe",
            ["정신증"] = "Psychosis",
            ["장난치기"] = "Puck Around",
            ["호박 폭탄"] = "Pumpkin Bomb",
            ["꼭두각시 허물"] = "Puppet Husk",
            ["결의"] = "Purpose",
            ["수상해지는 빛"] = "Purrrifying Light",
            ["방화광"] = "Pyromaniac",
            ["재빠른 손가락"] = "Quick Fingers",
            ["붉은 낫"] = "Rad Scythe",
            ["광휘의 열쇠"] = "Radiant Key",
            ["빗소리"] = "Rain Sounds",
            ["썩은 칼날"] = "Rancid Blade",
            ["쥐"] = "Rat",
            ["쥐의 선물"] = "Rat Gift",
            ["쥐 육포"] = "Rat Jerky",
            ["쥐덫"] = "Rat Trap",
            ["전투식량"] = "Rations",
            ["게걸스러움"] = "Ravenous",
            ["생식"] = "Raw Diet",
            ["폭발 직전"] = "Ready to Blow",
            ["수확 터렛"] = "Reaper Turret",
            ["재충전"] = "Recharge",
            ["무모한 돌진"] = "Reckless Rush",
            ["무모함"] = "Recklessness",
            ["빨간색 수정"] = "Red Crystal",
            ["후회"] = "Regret",
            ["안도"] = "Relief",
            ["수리 터렛"] = "Repair Turret",
            ["억눌린 분노"] = "Repressed Rage",
            ["연구 키트"] = "Research Kit",
            ["연구 막대"] = "Research Rod",
            ["휴식"] = "Rest",
            ["개조"] = "Retool",
            ["보복 터렛"] = "Retributor Turret",
            ["돌아온 칼날"] = "Return Stroke",
            ["의식 부지"] = "Ritual Site",
            ["굽기"] = "Roast",
            ["구운 간식"] = "Roasted Snack",
            ["구르는 죽음"] = "Rolling Death",
            ["마찰 화상"] = "Rope Burn",
            ["썩은 파이"] = "Rotten Pie",
            ["썩은 이쑤시개"] = "Rotten Toothpick",
            ["라운드하우스"] = "Roundhouse",
            ["잔해"] = "Rubble",
            ["꼬르륵 배"] = "Rumbly Tummy",
            ["녹슨 면도날"] = "Rusted Razor",
            ["녹슨 톱"] = "Rusty Saw",
            ["녹슨 삽"] = "Rusty Shovel",
            ["녹슨 수저"] = "Rusty Spoon",
            ["신성한 펜던트"] = "Sacred Pendant",
            ["희생"] = "Sacrifice",
            ["핏빛"] = "Sanguiny",
            ["소독제"] = "Santitizer",
            ["만족한 굼벵이"] = "Satisfied Grub",
            ["만족스러운 한 입"] = "Satisfying Nibble",
            ["흉터"] = "Scar Tissue",
            ["폐품 회수"] = "Scavenge",
            ["약탈자의 칼날"] = "Scavenger Blade",
            ["과학!"] = "Science!",
            ["과학자"] = "Scientist",
            ["뒤져보기"] = "Scrounge",
            ["대낫"] = "Scythe",
            ["탐색"] = "Search",
            ["조미료"] = "Seasoning",
            ["두번째 추측"] = "Second Guess",
            ["자기혐오"] = "Self Loathing",
            ["쉴드 차지"] = "Shield Charge",
            ["전기 충격"] = "Shock",
            ["삽"] = "Shovel",
            ["표창"] = "Shurikens",
            ["정신병자"] = "Sicko",
            ["사이드킥 터렛"] = "Sidekick Turret",
            ["끓어오르는 분노"] = "Simmering Rage",
            ["해골 열쇠"] = "Skeleton Key",
            ["침낭"] = "SLEEPING BAG",
            ["몽유병"] = "Sleepwalk",
            ["슬라임 열쇠"] = "Slime Pick",
            ["기습"] = "Sneak Attack",
            ["더러워진 메스"] = "Soiled Scalpel",
            ["스프 빵가루"] = "Soup Crumbs",
            ["스프 냄비"] = "Soup Pot",
            ["스파크"] = "Spark",
            ["굼벵이 소환"] = "Spawn Grubs",
            ["신생아"] = "Spawnlings",
            ["매운맛 수프"] = "Spicy Soup",
            ["매운맛 토스티"] = "Spicy Toasty",
            ["매운맛 트러플"] = "Spicy Truffle",
            ["뾰족한 성격"] = "Spiky Personality",
            ["피튀기는 전기톱"] = "Splatter Saw",
            ["피튀기는 벌레"] = "Splatter Worm",
            ["가시"] = "Splinter",
            ["쪼개지는 삽"] = "Splintering Shovel",
            ["폭신한 버클러"] = "Spongey Buckler",
            ["퍼뜨리기"] = "Spread Around",
            ["주식"] = "Staple Diet",
            ["말랑말랑해지기"] = "Stay Puffed",
            ["전투 자극제"] = "Stem Pack",
            ["막대기"] = "Stick",
            ["끈적한 점액"] = "Sticky Goo",
            ["끈적한 잔여물"] = "Sticky Residue",
            ["끈적한 톱"] = "Sticky Saw",
            ["석재 열쇠"] = "Stone pick",
            ["랜턴"] = "Storm Lantern",
            ["이상한 정동석"] = "Strange Geode",
            ["몸 풀기 의식"] = "Stretching Ritual",
            ["천재적 발상"] = "Stroke Of Genius",
            ["고집 센 검"] = "Stubborn Sword",
            ["흡입 군주"] = "Sucklord",
            ["영양제"] = "Supliments",
            ["감정 억제"] = "Suppress Emotions",
            ["달콤한 꿈"] = "Sweet Dream",
            ["달콤한 뿌리"] = "Sweet Root",
            ["달콤한 향기"] = "Sweet Scent",
            ["주사기"] = "Syringe",
            ["식사 예절"] = "Table Manners",
            ["매혹적인 토스티"] = "Tantalizing Toasty",
            ["깽판"] = "Tantrum",
            ["테이프"] = "Tape",
            ["전기충격기"] = "Taser",
            ["오물 선호"] = "Taste for Gunk",
            ["맛없는 트러플"] = "Tasteless Truffle",
            ["맛있는 열쇠"] = "Tasty Pick",
            ["맛있는 트러플"] = "Tasty Truffle",
            ["실험체"] = "Test Subject",
            ["변화"] = "The Becoming",
            ["초거대 청거스"] = "The Biggest Chungus",
            ["약한 피부"] = "Thin Skinned",
            ["심사숙고"] = "Thinking Ahead",
            ["갈증의 메이스"] = "Thirsty Mace",
            ["사려 깊은 유충"] = "Thoughtful Larvae",
            ["몸부림치는 탈출"] = "Thrashing Escape",
            ["기분좋은 찌릿함"] = "Tingles",
            ["팅키"] = "Tinkie",
            ["두꺼비"] = "Toad",
            ["두꺼비 주스"] = "Toad Juice",
            ["두꺼비 왕"] = "Toad King",
            ["택하지 않은 두꺼비"] = "Toad Not Taken",
            ["두꺼비 열쇠"] = "Toad Pick",
            ["토스티"] = "Toasty",
            ["토스티 케밥"] = "Toasty Kebab",
            ["도구"] = "Tool",
            ["이쑤시개"] = "Toothpick",
            ["횃불"] = "Torch",
            ["독성 팔다리"] = "Toxic Limb",
            ["트랩도어 거미"] = "Trapdoor Spider",
            ["트러플 사냥"] = "Truffle Hunt",
            ["트러플 돼지"] = "Truffle Pig",
            ["든든한 지포 라이터"] = "Trusty Zippo",
            ["뒤틀린 이쑤시개"] = "Twisted Toothpick",
            ["불안한 느낌"] = "Uneasy Feeling",
            ["불경한 계약"] = "Unholy Pact",
            ["불경한 수프"] = "Unholy Soup",
            ["시원찮은 톱"] = "Unreliable Saw",
            ["불안한 수프"] = "Unsettling  Soup",
            ["쓸모없는 빈 상자"] = "Useless Empty Box",
            ["두꺼비 방랑자"] = "Vagabond Toad",
            ["통"] = "Vat",
            ["노련한 칼날"] = "Veteran Blade",
            ["진동하는 트러플"] = "Vibrating Truffle",
            ["진동버섯"] = "Vibro Cap",
            ["동조의 메이스"] = "Vicarious Mace",
            ["사악한 기름"] = "Vile Oil",
            ["고약한 간식"] = "Vile Snack",
            ["잠시 대기"] = "Wait A Bit",
            ["따뜻한 느낌"] = "Warm Feeling",
            ["따뜻한 금형"] = "Warm Mould",
            ["밍밍한 수프"] = "Watery Soup",
            ["이상한 수프"] = "Weird Soup",
            ["기묘한 트러플"] = "Weird Truffle",
            ["촘촘한 봉합"] = "Well Knitted",
            ["만전의 휴식"] = "Well Rested",
            ["속삭여진 비밀"] = "Whispered Secret",
            ["작업대"] = "Workbench",
            ["벌레나무"] = "Wormwood",
            ["상처입은 돼지"] = "Wounded Piglet",
            ["레킹볼"] = "Wrecking ball",
            ["웜 쇄도"] = "Wurm Surge",
            ["피에 굶주린 도살자"] = "Bloodthirsty Butcher",
            ["실험의 산물"] = "Body of Work",
            ["충전"] = "Charge",
            ["플랫라인"] = "Flatline",
            ["점점 커지는 자신감"] = "Growing Confidence",
            ["돌머리"] = "Hard Headed",
            ["감염"] = "Infection",
            ["라자루스"] = "Lazarus",
            ["싸움 준비 완료!"] = "Ready To Scrap",
            ["간식 준비 완료!"] = "Ready To Snack",
            ["행동 개시"] = "Spring Into Action",
            ["거침없는"] = "Unstoppable",
            ["섬멸 로봇"] = "Cleaner Bot",
            ["산성 플라스크"] = "Acid Flask",
            ["섭리의 괴수"] = "All Seeing Abomination",
            ["앞치마"] = "Apron",
            ["아크 용접기"] = "Arc Welder",
            ["질나쁜 취향"] = "Bad Taste",
            ["Beetle 23"] = "Beetle 23",
            ["생체전기 박쥐"] = "Bioelectric Bat",
            ["열쇠 한 무더기"] = "Bunch of keys",
            ["명료함"] = "Clarity",
            ["고양이 쿠키"] = "Cookie Cat",
            ["배설물"] = "Excrement",
            ["손가락"] = "Finger",
            ["형광봉"] = "Glowsticks",
            ["점액 선물"] = "Goo Gift",
            ["오물 만찬"] = "Gunk Feast",
            ["해커봇"] = "Hackerbot",
            ["무거운 톱"] = "Heavy Saw",
            ["도움의 손길"] = "Helping Hand",
            ["국자"] = "Ladle",
            ["충성스러운 쥐"] = "Loyal Rat",
            ["성냥"] = "Matchstick",
            ["의료 보고서"] = "Medical Report",
            ["펜라이트"] = "Penlight",
            ["들이받기"] = "Ram",
            ["녹슨 망치"] = "Rusted Hammer",
            ["드라이버"] = "Screwdriver",
            ["버섯 실험"] = "Shroom Experiment",
            ["밥찌꺼기"] = "Slop",
            ["소일렌트 그린"] = "Soylent Green",
            ["포자 허파"] = "Spore Lung",
            ["포자"] = "Spores",
            ["이상한 열쇠"] = "Strange Key",
            ["터미널"] = "Terminal",
            ["위협 스캔"] = "Threat Scan",
            ["두꺼비 실험"] = "Toad Experiment",
            ["역겨운 뷔페"] = "Vile Buffet",
            ["후려치기"] = "Whack",
            ["보편적 피해 카드"] = "Universal Damage do-er",
            ["블릿츠 박스"] = "Box o' Blitz",
            ["발명가의 열쇠"] = "Inventor's Pick",
            ["냅킨"] = "Napkin",
            ["투박한 토치"] = "Clunky Blowtorch",
            ["예리한 눈"] = "Keen Eye",
            ["너그러움"] = "Generous",
            ["늘 지켜보는 눈"] = "Always Looking",
            ["괴물의 피"] = "Monster Blood",
            ["밧줄"] = "Rope",
            ["빵 한 덩이"] = "Loaf of Bread",
            ["어둠에 물든"] = "Touched By Gloom",
            ["고철 갑옷"] = "Armour of Scraps",
            ["미스터 스푼"] = "Mr Spoon",
            ["플랜 B"] = "Plan B",
            ["소금 한 꼬집"] = "Pinch of Salt",
            ["책임 전가"] = "Shift blame",
            ["자판기"] = "Vending Machine",
            ["쓰레기"] = "Trash",
            ["화학 화상"] = "Chemical Burn",
            ["독성 화상"] = "Toxic Burn",
            ["따귀"] = "Slap",
            ["청소기"] = "Hoover",
            ["주스곽"] = "Juice Box",
            ["설탕 덩어리 상자"] = "Box of Sugar Bombs",
        };

        public static string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            string trimmed = value.Trim();
            trimmed = trimmed.Trim('.', ',', ':', ';', '!', '?', '。', '、');

            return Map.TryGetValue(trimmed, out var original)
                ? original
                : value;
        }
    }

    internal static class TextWithLinksState
    {
        public static void LogFieldStatus()
        {
            Plugin.Logger.LogInfo("TextWithLinks direct member access version active.");
        }

        public static void Reset(TextWithLinks instance)
        {
            try
            {
                instance.m_selectedLink = -1;
                instance.m_lastSelectedLinkIndex = -1;
                instance.m_lastSelectedLinkWordLength = -1;
                instance.m_isHoveringObject = false;

                // 아래 세 개는 IntelliSense에 안 보이면 일단 빼도 됨.
                // instance.isShowingTooltip = false;
                // instance.isShowingCard = false;
                // instance.isShowingRewardPreview = false;
            }
            catch (Exception e)
            {
                Plugin.Logger.LogWarning($"Failed to reset TextWithLinks state: {e.GetType().Name}: {e.Message}");
            }
        }

        public static void ResetForDeselect(TextWithLinks instance)
        {
            try
            {
                instance.m_selectedLink = -1;
                instance.m_lastSelectedLinkIndex = -1;
                instance.m_lastSelectedLinkWordLength = -1;
                instance.m_isHoveringObject = false;
            }
            catch (Exception e)
            {
                Plugin.Logger.LogWarning($"Failed to reset TextWithLinks deselect state: {e.GetType().Name}: {e.Message}");
            }
        }

        public static void ResetCachedRangeOnly(TextWithLinks instance)
        {
            try
            {
                instance.m_lastSelectedLinkIndex = 0;
                instance.m_lastSelectedLinkWordLength = 0;
            }
            catch (Exception e)
            {
                Plugin.Logger.LogWarning($"Failed to reset TextWithLinks cached range: {e.GetType().Name}: {e.Message}");
            }
        }

        public static bool HasValidCachedCharacterRange(TextWithLinks instance)
        {
            try
            {
                var tmp = instance.m_TextMeshPro as TMPro.TMP_Text;
                if (tmp == null)
                    return false;

                tmp.ForceMeshUpdate(true, true);

                int start = instance.m_lastSelectedLinkIndex;
                int count = instance.m_lastSelectedLinkWordLength;

                if (start < 0 || count < 0)
                    return false;

                int characterCount = tmp.textInfo.characterCount;

                if (start + count > characterCount)
                    return false;

                return true;
            }
            catch (Exception e)
            {
                Plugin.Logger.LogWarning($"TextWithLinks state validation failed: {e.GetType().Name}: {e.Message}");
                return false;
            }
        }

        public static void FixHoverOverlay(TextWithLinks instance)
        {
            try
            {
                var hover = instance._hoverInstance as TMPro.TMP_Text;
                if (hover == null)
                    return;

                hover.enableWordWrapping = false;
                hover.overflowMode = TMPro.TextOverflowModes.Overflow;

                // 텍스트가 박스보다 길면 박스 폭을 늘림
                hover.ForceMeshUpdate(true, true);

                var rect = hover.rectTransform;
                var size = rect.sizeDelta;

                float preferredWidth = hover.preferredWidth + 20f;

                if (size.x < preferredWidth)
                {
                    size.x = preferredWidth;
                    rect.sizeDelta = size;
                }

                hover.ForceMeshUpdate(true, true);
            }
            catch (Exception e)
            {
                Plugin.Logger.LogWarning($"Failed to fix hover overlay: {e.GetType().Name}: {e.Message}");
            }
        }
    }

    [HarmonyPatch]
    internal static class TMPLinkTextAliasPatch
    {
        private static MethodBase TargetMethod()
        {
            Plugin.Logger.LogInfo("Searching for TMPro.TMP_LinkInfo.GetLinkText()...");

            var method = AccessTools.Method(typeof(TMPro.TMP_LinkInfo), "GetLinkText");

            if (method == null)
                throw new Exception("Could not find TMPro.TMP_LinkInfo.GetLinkText().");

            Plugin.Logger.LogInfo($"Targeting method: {method.FullDescription()}");
            return method;
        }

        private static void Postfix(ref string __result)
        {
            if (string.IsNullOrWhiteSpace(__result))
                return;

            var before = __result;
            var after = KeywordAlias.Normalize(before);

            if (before != after)
            {
                Plugin.Logger.LogInfo($"TMP link text alias: \"{before}\" -> \"{after}\"");
                __result = after;
            }
        }
    }

    [HarmonyPatch]
    internal static class TMPLinkIdLogPatch
    {
        private static MethodBase TargetMethod()
        {
            Plugin.Logger.LogInfo("Searching for TMPro.TMP_LinkInfo.GetLinkID()...");

            var method = AccessTools.Method(typeof(TMPro.TMP_LinkInfo), "GetLinkID");

            if (method == null)
                throw new Exception("Could not find TMPro.TMP_LinkInfo.GetLinkID().");

            Plugin.Logger.LogInfo($"Targeting method: {method.FullDescription()}");
            return method;
        }

        private static void Postfix(ref string __result)
        {
            if (string.IsNullOrWhiteSpace(__result))
                return;

            Plugin.Logger.LogInfo($"TMP link ID: \"{__result}\"");
        }
    }

    [HarmonyPatch(typeof(TextWithLinks), "DeselectLink")]
    internal static class TextWithLinksDeselectSafetyPatch
    {
        private static Exception Finalizer(TextWithLinks __instance, Exception __exception)
        {
            if (__exception == null)
                return null;

            if (ExceptionUtil.IsIndexOutOfRange(__exception))
            {
                Plugin.Logger.LogWarning("Suppressed TextWithLinks.DeselectLink index exception; resetting link state.");
                TextWithLinksState.ResetForDeselect(__instance);
                return null;
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(TextWithLinks), "ShowHoverOverlay")]
    internal static class TextWithLinksShowHoverOverlaySafetyPatch
    {
        private static Exception Finalizer(TextWithLinks __instance, Exception __exception)
        {
            // 예외가 없어도 hover overlay가 생성된 직후 줄바꿈/폭을 보정
            TextWithLinksState.FixHoverOverlay(__instance);

            if (__exception == null)
                return null;

            if (ExceptionUtil.IsIndexOutOfRange(__exception))
            {
                Plugin.Logger.LogWarning("Suppressed TextWithLinks.ShowHoverOverlay index exception; resetting cached range only.");
                TextWithLinksState.ResetCachedRangeOnly(__instance);
                return null;
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(TextWithLinks), "LateUpdate")]
    internal static class TextWithLinksLateUpdateSafetyPatch
    {
        private static Exception Finalizer(TextWithLinks __instance, Exception __exception)
        {
            if (__exception == null)
                return null;

            if (ExceptionUtil.IsIndexOutOfRange(__exception))
            {
                Plugin.Logger.LogWarning("Suppressed TextWithLinks.LateUpdate index exception; resetting cached range only.");
                TextWithLinksState.ResetCachedRangeOnly(__instance);
                return null;
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(TextWithLinks), "LateUpdate")]
    internal static class TextWithLinksRefreshPatch
    {
        private static void Prefix(TextWithLinks __instance)
        {
            try
            {
                var tmp = __instance.m_TextMeshPro as TMPro.TMP_Text;
                if (tmp == null)
                    return;

                // XUnity가 바꾼 TMP linkInfo/characterInfo를 hover 처리 전에 강제 재파싱
                tmp.SetVerticesDirty();
                tmp.SetLayoutDirty();
                tmp.ForceMeshUpdate(true, true);

                // 완전 reset은 하지 말고, 위험한 cached range만 안전값으로
                if (__instance.m_lastSelectedLinkIndex < 0 ||
                    __instance.m_lastSelectedLinkWordLength < 0 ||
                    __instance.m_lastSelectedLinkIndex + __instance.m_lastSelectedLinkWordLength > tmp.textInfo.characterCount)
                {
                    __instance.m_lastSelectedLinkIndex = 0;
                    __instance.m_lastSelectedLinkWordLength = 0;
                }
            }
            catch (Exception e)
            {
                Plugin.Logger.LogWarning($"TextWithLinks pre-refresh failed: {e.GetType().Name}: {e.Message}");
            }
        }
    }

    //[HarmonyPatch]
    //internal static class TMPTextSetTextPatch
    //{
    //    private static MethodBase TargetMethod()
    //    {
    //        Plugin.Logger.LogInfo("Searching for TMPro.TMP_Text.text setter...");

    //        var method = AccessTools.PropertySetter(typeof(TMPro.TMP_Text), "text");

    //        if (method == null)
    //            throw new Exception("Could not find TMPro.TMP_Text.text setter.");

    //        Plugin.Logger.LogInfo($"Targeting method: {method.FullDescription()}");
    //        return method;
    //    }

    //    private static void Prefix(ref string value)
    //    {
    //        if (string.IsNullOrEmpty(value))
    //            return;

    //        if (value.Contains("mC") || value.Contains("Well Rested") || value.Contains("HANDY"))
    //        {
    //            Plugin.Logger.LogInfo($"TMP set_text source: {value}");
    //        }

    //        if (value.Contains("link=mC") || value.Contains("link\\=mC") || value.Contains("<link=mC") || value.Contains("<link\\=mC"))
    //        {
    //            var before = value;
    //            value = McCardNameLocalizer.LocalizeMcLinks(value);

    //            if (before != value)
    //                Plugin.Logger.LogInfo($"TMP set_text localized: {value}");
    //        }
    //    }
    //}
    [HarmonyPatch(typeof(TextWithLinks), "LateUpdate")]
    internal static class TextWithLinksLinkLocalizePatch
    {
        private static readonly Dictionary<IntPtr, string> LastText = new();

        private static void Prefix(TextWithLinks __instance)
        {
            try
            {
                var tmp = __instance.m_TextMeshPro as TMPro.TMP_Text;
                if (tmp == null)
                    return;

                string text = tmp.text;
                if (string.IsNullOrEmpty(text))
                    return;

                var key = __instance.Pointer;

                if (LastText.TryGetValue(key, out var previous) && previous == text)
                    return;

                LastText[key] = text;

                if (!text.Contains("<link"))
                    return;

                string localized = LinkTextLocalizer.LocalizeAllLinks(text);

                if (localized == text)
                    return;

                Plugin.Logger.LogInfo("Localizing link text inside TextWithLinks text.");

                tmp.text = localized;
                tmp.SetVerticesDirty();
                tmp.SetLayoutDirty();
                tmp.ForceMeshUpdate(true, true);

                __instance.m_selectedLink = -1;
                __instance.m_lastSelectedLinkIndex = 0;
                __instance.m_lastSelectedLinkWordLength = 0;
                __instance.m_isHoveringObject = false;

                LastText[key] = localized;
            }
            catch (Exception e)
            {
                Plugin.Logger.LogWarning($"TextWithLinks link localization failed: {e.GetType().Name}: {e.Message}");
            }
        }
    }


    internal static class LinkTextLocalizer
    {
        private static readonly Dictionary<string, string> TextMap = new()
        {
            ["Creature"] = "생명체",
            ["Food"] = "음식",
            ["Skill"] = "스킬",
            ["None"] = "없음",
            ["Passive"] = "패시브",
            ["Attack"] = "공격",
            ["Mood"] = "기분",
            ["Curse"] = "저주",
            ["Machine"] = "기계",
            ["Charge"] = "충전",
            ["Data"] = "데이터",
            ["Camp"] = "캠프",
            ["Pick"] = "열쇠",
            ["Search"] = "탐색",
            ["Death"] = "죽음",
            ["Special"] = "특수",
            ["Foresight"] = "예견",
            ["IF FATAL"] = "처치 시",
            ["IF GUNK IS REMOVED on this card"] = "오물 제거 시",
            ["FOR EVERY GUNK REMOVED"] = "제거한 오물마다",
            ["IF FULLY UNGUNKED"] = "완전히 오물 제거 시",
            ["IF UNGUNKING CARD"] = "카드에서 오물 제거 시",
            ["IF STORED GUNK CONSUMED"] = "저장된 오물 소모 시",
            ["IF GUNK REMOVED on target"] = "오물 제거 시",
            ["IF ANY CARD GUNKED"] = "오물투성이 카드가 있을 시",
            ["IF ALL GUNK REMOVED on any card"] = "모든 오물 제거 시",
            ["IF GUNKED"] = "오물투성이일 시",
            ["WHEN OVERCHARGED"] = "과충전 시",
            ["WHEN OVERCHARGE ENDS"] = "과충전 종료 시",
            ["WHEN ANY CARD OVERCHARGES"] = "카드가 과충전될 시",
            ["WHEN ANY OVERCHARGE ENDS"] = "과충전이 종료될 시",
            ["TOAD"] = "두꺼비",
            ["EAT"] = "섭취",
            ["CHARGE"] = "충전",
            ["STRUCTURE"] = "구조물",
            ["ODD"] = "이상한",
            ["STORE"] = "저장",
            ["GUNKY"] = "오물투성이",
            ["GEAR"] = "톱니바퀴",
            ["EXILE"] = "추방",
            ["CAST"] = "시전",
            ["DISCARD"] = "버리기",
            ["EXHAUST"] = "소멸",
            ["STORE PROTEIN"] = "단백질을 저장",
            ["SWALLOW"] = "섭식",
            ["DISCHARGE"] = "방전",
            ["CHARGED"] = "충전됨",
            ["CONSUME"] = "소모",
            ["CRIPPLE"] = "무력화",
            ["LAY A SPIDER EGG"] = "거미 알을 낳습니다",
            ["CALL A PET"] = "펫을 부릅니다",
            ["FAT GRUB"] = "뚱뚱한 굼벵이",
            ["TRUFFLE PIG"] = "트러플 돼지",
            ["STICKY GOO"] = "끈적한 점액",
            ["OR"] = "혹은",
            ["AND"] = "그리고",
            ["BREAKABLE"] = "파괴 가능",
            ["DIGGABLE"] = "발굴 가능",
            ["FEEDABLE"] = "먹이주기 가능",
            ["IGNITABLE"] = "점화 가능",
            ["FIXABLE"] = "수리 가능",
            ["REVEALABLE"] = "공개 가능",
            ["UNLOCKABLE"] = "개방 가능",
            ["WHEN USED"] = "사용할 때",
            ["Protein"] = "단백질",
            ["Unlock"] = "개방",
            ["Dig"] = "발굴",
            ["Break"] = "파괴",
            ["Fix"] = "수리",
            ["Reveal"] = "공개",
            ["Ignite"] = "점화",
            ["Feed"] = "먹이주기",
            ["Meditate"] = "명상",
            ["Nothing"] = "없음",
            ["Absorb"] = "흡수",
            ["AMPLIFY GLOOM"] = "어둠 증폭",
            ["Analog"] = "아날로그",
            ["Any"] = "아무",
            ["Battery"] = "배터리",
            ["Camp"] = "캠프",
            ["Coupon"] = "쿠폰",
            ["Data"] = "데이터",
            ["Deathless"] = "불사",
            ["Nervous"] = "긴장된",
            ["Shroom"] = "버섯 난",
            ["Singed"] = "그을린",
            ["Weird"] = "이상한",
            ["Wild"] = "야성적인",
            ["Energize"] = "활력",
            ["Energy Cost"] = "에너지 비용",
            ["Evolve"] = "진화",
            ["Exhaust"] = "소멸",
            ["Warmth"] = "온기",
            ["Food"] = "음식",
            ["Fragile"] = "취급주의",
            ["Gold"] = "골드",
            ["Draw With Gunk"] = "오물을 추가하여 뽑기",
            ["Gunk"] = "오물",
            ["Stored Gunk"] = "저장된 오물",
            ["Thirsty"] = "목마름",
            ["Handy"] = "능숙",
            ["Horror"] = "공포",
            ["If Fatal Trigger"] = "처치 시 트리거",
            ["Inescapable"] = "불가피",
            ["Linger"] = "보존",
            ["Mood"] = "기분",
            ["Persistent"] = "지속",
            ["Rage"] = "격노",
            ["Replenish"] = "보충",
            ["Sterile"] = "멸균",
            ["Gloom"] = "어둠",
            ["Temporary"] = "임시",
            ["Unplayable"] = "사용 불가",
            ["Uses"] = "남은 횟수:",
            ["Permanent"] = "영구",
            ["FIXABLE"] = "수리 가능",
            ["BREAKABLE"] = "파괴 가능",
            ["DIGGABLE"] = "발굴 가능",
            ["FEEDABLE"] = "먹이주기 가능",
            ["IGNITABLE"] = "발화 가능",
            ["REVEALABLE"] = "공개 가능",
            ["UNLOCKABLE"] = "개방 가능",
            ["Explore"] = "탐험",
            ["Combat"] = "전투",
            ["Foresight"] = "예견",

            ["Psychosis"] = "정신증",
            ["Liver Wort"] = "간이끼",
            ["Horn Wort"] = "뿔이끼",
            ["Crusty Curse"] = "찌든때의 저주",
            ["Loving"] = "애정 어린",
            ["Charged"] = "충전됨",
            ["Conductive"] = "전도성",
            ["Evolving"] = "진화하는",
            ["Gloom Creature"] = "음울의 피조물",
            ["Gutsy"] = "배짱 있는",
            ["Irritable"] = "화가 난",
            ["Limb"] = "팔다리",
            ["Loot"] = "전리품",
            ["Moody"] = "변덕스러운",
            ["Mossy"] = "이끼 낀",
            ["Scaredy"] = "겁쟁이",
            ["Slimey"] = "점액질",
            ["Team Player"] = "팀 플레이어",
            ["Thorns"] = "가시",
            ["Airborne"] = "비행",
            ["Armoured"] = "장갑",
            ["Bless"] = "축복",
            ["Brainless"] = "무지성",
            ["Buzzkill"] = "흐름 끊기",
            ["Lodged"] = "부착",
            ["Confused"] = "혼란",
            ["Crippled"] = "무력화",
            ["Deathless"] = "불사",
            ["Decay"] = "부패",
            ["Dividing"] = "분열 중",
            ["Enrage"] = "격노",
            ["Ethereal"] = "영체 상태",
            ["Fruiting"] = "결실 맺기",
            ["Gestating"] = "잉태 중",
            ["Hot Sauce"] = "핫소스",
            ["Infested"] = "감염됨",
            ["Insight"] = "통찰",
            ["Inspired"] = "고무됨",
            ["Regrowth"] = "재생",
            ["Resurgence"] = "재기",
            ["Rotten"] = "썩은",
            ["Seasoned"] = "양념",
            ["Slime Mom"] = "슬라임 엄마",
            ["Spell Weakness"] = "주문 취약",
            ["Spellshield"] = "주문 보호막",
            ["Sporey"] = "포자투성이의",
            ["Starving"] = "굶주림",
            ["Strange Exit"] = "기이한 출구",
            ["Stun"] = "기절",
            ["Swift"] = "신속함",
            ["Thorned"] = "가시 돋친",
            ["Timid"] = "소심함",
            ["Unnerving"] = "불안감을 주는",
            ["Vile"] = "비열한",
            ["Vulnerable"] = "취약함",

            ["Stab"] = "찌르기",
            ["2M Rope"] = "2M 밧줄",
            ["Absorb"] = "흡수",
            ["Acacia Mace"] = "아카시아 철퇴",
            ["Afterglow"] = "잔광",
            ["All seeing eye"] = "섭리의 눈",
            ["Analysis"] = "분석",
            ["Assasinate"] = "암살",
            ["Auto Saw"] = "전동 톱",
            ["Avenger"] = "복수자",
            ["Awesome Soup"] = "끝내주는 수프",
            ["Back Scratch"] = "등 긁기",
            ["Backpack"] = "배낭",
            ["Bad Beef"] = "상한 소고기",
            ["Bag Of Picks"] = "열쇠 주머니",
            ["Bag of Secrets"] = "비밀 주머니",
            ["Bandages"] = "붕대",
            ["Bandaid"] = "반창고",
            ["Baptise"] = "세례",
            ["Barbed Vine"] = "가시 돋친 덩굴",
            ["Barf Pick"] = "토사물 열쇠",
            ["Bash"] = "강타",
            ["Bat Pack"] = "박쥐 열쇠",
            ["Beamer"] = "대형 손전등",
            ["Big Gulp"] = "시원한 한 잔",
            ["Bile Blast"] = "담즙 발사",
            ["Bio Forge"] = "생체 제련소",
            ["Biofuel Generator"] = "바이오연료 발전기",
            ["Bite"] = "물어뜯기",
            ["Bitter Medicine"] = "입에 쓴 약",
            ["Bizzare Toasty"] = "기묘한 토스티",
            ["Bladed Escape"] = "날카로운 탈출",
            ["Blessed Toasty"] = "축복의 토스티",
            ["Blitz"] = "블릿츠",
            ["Blocking Hammer"] = "방어 망치",
            ["Blood Bath"] = "피바다",
            ["Blowtorch"] = "토치",
            ["Blue Crystal"] = "파란색 수정",
            ["Bolts"] = "볼트",
            ["Bone Saw"] = "뼈 톱",
            ["Bone Spur"] = "뼈 가시",
            ["Bone To Pick"] = "뼈다귀 열쇠",
            ["Box of Donuts"] = "도넛 상자",
            ["Box of Matches"] = "성냥 상자",
            ["Box of Tinkies"] = "팅키 상자",
            ["Bread Crumbs"] = "빵가루",
            ["Briar Hammer"] = "가시덤불 망치",
            ["Buff Fingers"] = "근육질 손가락",
            ["Bug Attack"] = "벌레 공격",
            ["Bunsen Burner"] = "분젠 버너",
            ["Burning Rage"] = "불타는 분노",
            ["Burning Wine"] = "불타는 와인",
            ["Bursting Butts"] = "터지는 벌레",
            ["Calculator"] = "계산기",
            ["Call for Blood"] = "피의 부름",
            ["Camp Fire"] = "캠프파이어",
            ["Canary"] = "카나리아",
            ["Careful Planning"] = "신중한 계획",
            ["Carrot"] = "당근",
            ["Catharsis"] = "카타르시스",
            ["Cell"] = "건전지",
            ["Charred Toasty"] = "까맣게 탄 토스티",
            ["Cheap Lighter"] = "싸구려 라이터",
            ["Chewed Gum"] = "씹던 껌",
            ["Chewing Gristle"] = "연골 씹기",
            ["Churning Turret"] = "휘젓는 터렛",
            ["Cleaning Supplies"] = "청소 도구",
            ["Clear Head"] = "맑은 정신",
            ["Close Friend"] = "영혼의 단짝",
            ["Clunker"] = "고물",
            ["Coffee Station"] = "커피 스테이션",
            ["Coffin"] = "관짝",
            ["Commander"] = "지휘관",
            ["Constant Rage"] = "끝없는 분노",
            ["Corrupted Limb"] = "타락한 팔다리",
            ["Cosmic Horror"] = "우주적 공포",
            ["Coupon"] = "쿠폰",
            ["Creature"] = "생명체",
            ["Crippling Blow"] = "무력화 타격",
            ["Crud Cookie"] = "찌꺼기 쿠키",
            ["Crud Turret"] = "찌꺼기 터렛",
            ["Crusted Blade"] = "때에 찌든 칼날",
            ["Crusty Curse"] = "찌든때의 저주",
            ["Crystal Ball"] = "수정구",
            ["Cup of Cocoa"] = "핫초코 한 잔",
            ["Cup of Coffee"] = "커피 한 잔",
            ["Curious Bug"] = "호기심많은 벌레",
            ["Cutlery"] = "식기 세트",
            ["Dark Acquaintance"] = "어둠의 조력자",
            ["Dark Crystal"] = "어둠의 수정",
            ["Data"] = "데이터",
            ["Deadly Mould"] = "치명적인 곰팡이",
            ["Death Plunger"] = "죽움의 뚜러뻥",
            ["Death Stare"] = "죽음의 시선",
            ["Dented Thermous"] = "찌그러진 보온병",
            ["Desperation"] = "절박함",
            ["Destroyer"] = "파괴자",
            ["Detective"] = "탐정",
            ["Dice"] = "주사위",
            ["Dirt Nap"] = "영면",
            ["Dirty Splinter"] = "더러운 가시",
            ["Discipline"] = "규율",
            ["Dissection Table"] = "해부대",
            ["Dissolving Juices"] = "소화 주스",
            ["Distant Shadow"] = "아득한 그림자",
            ["DJ Boomba"] = "DJ 붐바",
            ["Donut"] = "도넛",
            ["Dread"] = "공포",
            ["Drill"] = "드릴",
            ["Echoing Flute"] = "메아리치는 피리",
            ["Electro Prod"] = "전기 꼬챙이",
            ["Emotional Rollercoaster"] = "감정의 롤러코스터",
            ["Employee Handbook"] = "직무 참고서",
            ["Empty Jar"] = "빈 항아리",
            ["Encouragement"] = "격려",
            ["Endless Larvae"] = "끝없는 유충",
            ["Endless Soup"] = "끝없는 수프",
            ["eRADIATE"] = "방사(능)",
            ["EUREKA!"] = "유레카!",
            ["Everlasting Truffle"] = "영원의 트러플",
            ["Extra Padding"] = "누더기 덧댐",
            ["Eyeball"] = "눈알",
            ["Familiarity"] = "익숙함",
            ["Fan Of Pain"] = "고통 애호가",
            ["Fat Grub"] = "통통한 굼벵이",
            ["Fatigue"] = "피로",
            ["Feeling Handy"] = "손재주",
            ["Feral Charge"] = "야성적인 돌격",
            ["Fertile Growth"] = "풍요로운 성장",
            ["Festering Mound"] = "썩어가는 더미",
            ["Fever"] = "고열",
            ["Fiery Soup"] = "불타는 수프",
            ["Filthy Habit"] = "더러운 습관",
            ["Firecrackers"] = "폭죽",
            ["Fixer"] = "정비공",
            ["Flaccid Sword"] = "흐릿한 검",
            ["Flagellate"] = "채찍질",
            ["Flambee"] = "플람베",
            ["Flame Turret"] = "화염 터렛",
            ["Flaming Fork"] = "불타는 포크",
            ["Flare"] = "조명탄",
            ["Flash"] = "플래시",
            ["Flash Turret"] = "섬광 터렛",
            ["Flashlight"] = "손전등",
            ["Flint"] = "부싯돌",
            ["Fly kick"] = "날아차기",
            ["Focus"] = "집중",
            ["Foodworm"] = "음식벌레",
            ["Foraged Snack"] = "채집한 간식",
            ["Forced Growth"] = "강제 성장",
            ["Foreboding"] = "예감",
            ["Fortune"] = "행운",
            ["Foul Breath"] = "악취나는 숨결",
            ["Foul Vegetation"] = "썩어가는 덤불",
            ["Frantic Search"] = "필사적인 수색",
            ["Friendly Ember"] = "친근한 불씨",
            ["Fruiting Limb"] = "증식하는 팔다리",
            ["Frustration"] = "짜증",
            ["Fumble slash"] = "헛손질 베기",
            ["Fungal Bread"] = "균류 빵",
            ["Fungal Loaf"] = "균류 덩어리빵",
            ["Go-To Snack"] = "최애 간식",
            ["Golden Pollen"] = "황금 꽃가루",
            ["Gouge"] = "파내기",
            ["Gourmet Toasty"] = "미식의 토스티",
            ["Grand Maul"] = "거대한 곤봉",
            ["Grease"] = "윤활유",
            ["Greedy Dagger"] = "탐욕의 단검",
            ["Green Crystal"] = "초록색 수정",
            ["Grimoire"] = "마도서",
            ["Gristle Spit"] = "연골 뱉기",
            ["Growing Soup"] = "촉진의 수프",
            ["Grubby Fingers"] = "꼬질꼬질한 손가락",
            ["Grubs"] = "굼벵이들",
            ["Grudge"] = "원한",
            ["Gunk Blaster"] = "오물 발사기",
            ["Gunk Harvester"] = "오물 수확기",
            ["Gunk Lung"] = "오물 허파",
            ["Gunk Toaster"] = "오물 토스터",
            ["Gunka Cola"] = "오물 콜라",
            ["Gunkpocalypse"] = "오포칼립스",
            ["Gunky Grabber"] = "오물투성이 가젯팔",
            ["Guts"] = "내장",
            ["Hammer"] = "망치",
            ["Hazmat Suit"] = "방호복",
            ["Hearty Soup"] = "든든한 수프",
            ["Hollow Hamster"] = "공허한 햄스터",
            ["Holy Grub"] = "성스러운 굼벵이",
            ["Holy Soup"] = "성스러운 수프",
            ["Hope"] = "한 줄기 희망",
            ["Hornwart"] = "뿔이끼",
            ["Horrible Club"] = "끔찍한 몽둥이",
            ["Hot Poker"] = "달궈진 불쏘시개",
            ["Hot Sauce"] = "핫소스",
            ["Hot Toasty"] = "뜨거운 토스티",
            ["Hug Attack"] = "포옹 공격",
            ["Hungry blade"] = "굶주린 칼날",
            ["Indecision"] = "선택장애",
            ["Infected Limb"] = "감염된 팔다리",
            ["Infected Saw"] = "감염된 톱",
            ["Inferno"] = "불지옥",
            ["Insatiable Grub"] = "탐욕스러운 굼벵이",
            ["Inspiration"] = "영감",
            ["Inspiring Pick"] = "영감의 열쇠",
            ["Insulation"] = "단열재",
            ["Inventors blade"] = "발명가의 칼날",
            ["Itchy Fingers"] = "가려운 손가락",
            ["Jabber"] = "찌르개",
            ["Junk Food"] = "정크푸드",
            ["Key"] = "열쇠",
            ["Key Card"] = "키 카드",
            ["Kitchen"] = "주방",
            ["Lazy Fingers"] = "게으른 손가락",
            ["Left To Rot"] = "썩어 문드러진 것",
            ["Leftovers"] = "남은 식량",
            ["Lick"] = "핥기",
            ["Limber Escape"] = "유연한 탈출",
            ["Lingering Warmth"] = "남아있는 온기",
            ["Lock Breaker"] = "자물쇠 파괴자",
            ["Lock Eater"] = "자물쇠 포식자",
            ["Lock Flail"] = "자물쇠 도리깨",
            ["Lock Pig"] = "락피그",
            ["Lock Slimer"] = "자물쇠 점액 도포기",
            ["Lockpick"] = "열쇠",
            ["Lost Soul"] = "길 잃은 영혼",
            ["Loving parasite"] = "사랑스러운 기생충",
            ["Loyal Canary"] = "충성스러운 카나리아",
            ["Loyal Piglet"] = "충성그러운 새끼돼지",
            ["Lubricated"] = "미끈거림",
            ["Map"] = "지도",
            ["Marshmallows"] = "마시멜로",
            ["Medibot"] = "구급로봇",
            ["Medical Kit"] = "구급상자",
            ["Meditate"] = "명상",
            ["Messy Blowtorch"] = "꾀죄죄한 화염귀",
            ["Micro Turret"] = "초소형 터렛",
            ["Minor Growth"] = "미미한 성장",
            ["Molechild"] = "어린 두더쥐",
            ["Monster Juice"] = "괴물 주스",
            ["Morning Stretches"] = "아침 스트레칭",
            ["Mossy Rations"] = "이끼 낀 전투식량",
            ["Mossy Snack"] = "이끼 간식",
            ["Motivated"] = "의욕 충만",
            ["Moult"] = "털갈이",
            ["Muligan"] = "멀리건",
            ["Muscle Mallet"] = "중량 망치",
            ["Muscle Spasm"] = "근육 경련",
            ["Mushroom Key"] = "버섯 열쇠",
            ["Mutant Pick"] = "돌연변이 열쇠",
            ["Mutant Smash"] = "돌연변이 강타",
            ["Nail"] = "못",
            ["Notebook"] = "공책",
            ["Nurti Fists"] = "영양 주먹",
            ["Nutrient Fabricator"] = "영양분 합성기",
            ["Odd Rock"] = "이상한 돌",
            ["Ooze"] = "분비물",
            ["Oozing Canister"] = "질척이는 용기",
            ["Oozing Flesh"] = "질척이는 살덩이",
            ["Outburst"] = "격분",
            ["Over Do It"] = "무리하기",
            ["Pack O cELLS"] = "건전지 한 묶음",
            ["Pact"] = "계약",
            ["Padkos"] = "패드코스",
            ["Painkillers"] = "진통제",
            ["Panic"] = "공황",
            ["Personal Growth"] = "개인적 성장",
            ["Pick Axe"] = "곡괭이",
            ["Picky Eater"] = "편식쟁이",
            ["Pig Louse"] = "돼지 기생충",
            ["Pig Pen"] = "돼지우리",
            ["Pipette"] = "피펫",
            ["Pissed Off"] = "열받음",
            ["Pliers"] = "펜치",
            ["PMA"] = "긍정적 마음가짐",
            ["Pocket Lint"] = "보풀 덩어리",
            ["Possibility Crystal"] = "가능성의 수정",
            ["Power Nap"] = "파워 낮잠",
            ["Powernap"] = "파워 낮잠",
            ["Practice Dummy"] = "훈련용 허수아비",
            ["Precious Morsel"] = "소중한 한입",
            ["Probe"] = "탐침",
            ["Psychosis"] = "정신증",
            ["Puck Around"] = "장난치기",
            ["Pumpkin Bomb"] = "호박 폭탄",
            ["Puppet Husk"] = "꼭두각시 허물",
            ["Purpose"] = "결의",
            ["Purrrifying Light"] = "수상해지는 빛",
            ["Pyromaniac"] = "방화광",
            ["Quick Fingers"] = "재빠른 손가락",
            ["Rad Scythe"] = "붉은 낫",
            ["Radiant Key"] = "광휘의 열쇠",
            ["Rain Sounds"] = "빗소리",
            ["Rancid Blade"] = "썩은 칼날",
            ["Rat"] = "쥐",
            ["Rat Gift"] = "쥐의 선물",
            ["Rat Jerky"] = "쥐 육포",
            ["Rat Trap"] = "쥐덫",
            ["Rations"] = "전투식량",
            ["Ravenous"] = "게걸스러움",
            ["Raw Diet"] = "생식",
            ["Ready to Blow"] = "폭발 직전",
            ["Reaper Turret"] = "수확 터렛",
            ["Recharge"] = "재충전",
            ["Reckless Rush"] = "무모한 돌진",
            ["Recklessness"] = "무모함",
            ["Red Crystal"] = "빨간색 수정",
            ["Regret"] = "후회",
            ["Relief"] = "안도",
            ["Repair Turret"] = "수리 터렛",
            ["Repressed Rage"] = "억눌린 분노",
            ["Research Kit"] = "연구 키트",
            ["Research Rod"] = "연구 막대",
            ["Rest"] = "휴식",
            ["Retool"] = "개조",
            ["Retributor Turret"] = "보복 터렛",
            ["Return Stroke"] = "돌아온 칼날",
            ["Ritual Site"] = "의식 부지",
            ["Roast"] = "굽기",
            ["Roasted Snack"] = "구운 간식",
            ["Rolling Death"] = "구르는 죽음",
            ["Rope Burn"] = "마찰 화상",
            ["Rotten Pie"] = "썩은 파이",
            ["Rotten Toothpick"] = "썩은 이쑤시개",
            ["Roundhouse"] = "라운드하우스",
            ["Rubble"] = "잔해",
            ["Rumbly Tummy"] = "꼬르륵 배",
            ["Rusted Razor"] = "녹슨 면도날",
            ["Rusty Saw"] = "녹슨 톱",
            ["Rusty Shovel"] = "녹슨 삽",
            ["Rusty Spoon"] = "녹슨 수저",
            ["Sacred Pendant"] = "신성한 펜던트",
            ["Sacrifice"] = "희생",
            ["Sanguiny"] = "핏빛",
            ["Santitizer"] = "소독제",
            ["Satisfied Grub"] = "만족한 굼벵이",
            ["Satisfying Nibble"] = "만족스러운 한 입",
            ["Scar Tissue"] = "흉터",
            ["Scavenge"] = "폐품 회수",
            ["Scavenger Blade"] = "약탈자의 칼날",
            ["Science!"] = "과학!",
            ["Scientist"] = "과학자",
            ["Scrounge"] = "뒤져보기",
            ["Scythe"] = "대낫",
            ["Search"] = "탐색",
            ["Seasoning"] = "조미료",
            ["Second Guess"] = "두번째 추측",
            ["Self Loathing"] = "자기혐오",
            ["Shield Charge"] = "쉴드 차지",
            ["Shock"] = "전기 충격",
            ["Shovel"] = "삽",
            ["Shurikens"] = "표창",
            ["Sicko"] = "정신병자",
            ["Sidekick Turret"] = "사이드킥 터렛",
            ["Simmering Rage"] = "끓어오르는 분노",
            ["Skeleton Key"] = "해골 열쇠",
            ["SLEEPING BAG"] = "침낭",
            ["Sleepwalk"] = "몽유병",
            ["Slime Pick"] = "슬라임 열쇠",
            ["Sneak Attack"] = "기습",
            ["Soiled Scalpel"] = "더러워진 메스",
            ["Soup Crumbs"] = "스프 빵가루",
            ["Soup Pot"] = "스프 냄비",
            ["Spark"] = "스파크",
            ["Spawn Grubs"] = "굼벵이 소환",
            ["Spawnlings"] = "신생아",
            ["Spicy Soup"] = "매운맛 수프",
            ["Spicy Toasty"] = "매운맛 토스티",
            ["Spicy Truffle"] = "매운맛 트러플",
            ["Spiky Personality"] = "뾰족한 성격",
            ["Splatter Saw"] = "피튀기는 전기톱",
            ["Splatter Worm"] = "피튀기는 벌레",
            ["Splinter"] = "가시",
            ["Splintering Shovel"] = "쪼개지는 삽",
            ["Spongey Buckler"] = "폭신한 버클러",
            ["Spread Around"] = "퍼뜨리기",
            ["Staple Diet"] = "주식",
            ["Stay Puffed"] = "말랑말랑해지기",
            ["Stem Pack"] = "전투 자극제",
            ["Stick"] = "막대기",
            ["Sticky Goo"] = "끈적한 점액",
            ["Sticky Residue"] = "끈적한 잔여물",
            ["Sticky Saw"] = "끈적한 톱",
            ["Stone pick"] = "석재 열쇠",
            ["Storm Lantern"] = "랜턴",
            ["Strange Geode"] = "이상한 정동석",
            ["Stretching Ritual"] = "몸 풀기 의식",
            ["Stroke Of Genius"] = "천재적 발상",
            ["Stubborn Sword"] = "고집 센 검",
            ["Sucklord"] = "흡입 군주",
            ["Supliments"] = "영양제",
            ["Suppress Emotions"] = "감정 억제",
            ["Sweet Dream"] = "달콤한 꿈",
            ["Sweet Root"] = "달콤한 뿌리",
            ["Sweet Scent"] = "달콤한 향기",
            ["Syringe"] = "주사기",
            ["Table Manners"] = "식사 예절",
            ["Tantalizing Toasty"] = "매혹적인 토스티",
            ["Tantrum"] = "깽판",
            ["Tape"] = "테이프",
            ["Taser"] = "전기충격기",
            ["Taste for Gunk"] = "오물 선호",
            ["Tasteless Truffle"] = "맛없는 트러플",
            ["Tasty Pick"] = "맛있는 열쇠",
            ["Tasty Truffle"] = "맛있는 트러플",
            ["Test Subject"] = "실험체",
            ["The Becoming"] = "변화",
            ["The Biggest Chungus"] = "초거대 청거스",
            ["Thin Skinned"] = "약한 피부",
            ["Thinking Ahead"] = "심사숙고",
            ["Thirsty Mace"] = "갈증의 메이스",
            ["Thoughtful Larvae"] = "사려 깊은 유충",
            ["Thrashing Escape"] = "몸부림치는 탈출",
            ["Tingles"] = "기분좋은 찌릿함",
            ["Tinkie"] = "팅키",
            ["Toad"] = "두꺼비",
            ["Toad Juice"] = "두꺼비 주스",
            ["Toad King"] = "두꺼비 왕",
            ["Toad Not Taken"] = "택하지 않은 두꺼비",
            ["Toad Pick"] = "두꺼비 열쇠",
            ["Toasty"] = "토스티",
            ["Toasty Kebab"] = "토스티 케밥",
            ["Tool"] = "도구",
            ["Toothpick"] = "이쑤시개",
            ["Torch"] = "횃불",
            ["Toxic Limb"] = "독성 팔다리",
            ["Trapdoor Spider"] = "트랩도어 거미",
            ["Truffle Hunt"] = "트러플 사냥",
            ["Truffle Pig"] = "트러플 돼지",
            ["Trusty Zippo"] = "든든한 지포 라이터",
            ["Twisted Toothpick"] = "뒤틀린 이쑤시개",
            ["Uneasy Feeling"] = "불안한 느낌",
            ["Unholy Pact"] = "불경한 계약",
            ["Unholy Soup"] = "불경한 수프",
            ["Unreliable Saw"] = "시원찮은 톱",
            ["Unsettling  Soup"] = "불안한 수프",
            ["Useless Empty Box"] = "쓸모없는 빈 상자",
            ["Vagabond Toad"] = "두꺼비 방랑자",
            ["Vat"] = "통",
            ["Veteran Blade"] = "노련한 칼날",
            ["Vibrating Truffle"] = "진동하는 트러플",
            ["Vibro Cap"] = "진동버섯",
            ["Vicarious Mace"] = "동조의 메이스",
            ["Vile Oil"] = "사악한 기름",
            ["Vile Snack"] = "고약한 간식",
            ["Wait A Bit"] = "잠시 대기",
            ["Warm Feeling"] = "따뜻한 느낌",
            ["Warm Mould"] = "따뜻한 금형",
            ["Watery Soup"] = "밍밍한 수프",
            ["Weird Soup"] = "이상한 수프",
            ["Weird Truffle"] = "기묘한 트러플",
            ["Well Knitted"] = "촘촘한 봉합",
            ["Well Rested"] = "만전의 휴식",
            ["Whispered Secret"] = "속삭여진 비밀",
            ["Workbench"] = "작업대",
            ["Wormwood"] = "벌레나무",
            ["Wounded Piglet"] = "상처입은 돼지",
            ["Wrecking ball"] = "레킹볼",
            ["Wurm Surge"] = "웜 쇄도",
            ["Bloodthirsty Butcher"] = "피에 굶주린 도살자",
            ["Body of Work"] = "실험의 산물",
            ["Charge"] = "충전",
            ["Flatline"] = "플랫라인",
            ["Growing Confidence"] = "점점 커지는 자신감",
            ["Hard Headed"] = "돌머리",
            ["Infection"] = "감염",
            ["Lazarus"] = "라자루스",
            ["Ready To Scrap"] = "싸움 준비 완료!",
            ["Ready To Snack"] = "간식 준비 완료!",
            ["Spring Into Action"] = "행동 개시",
            ["Unstoppable"] = "거침없는",
            ["Cleaner Bot"] = "섬멸 로봇",
            ["Acid Flask"] = "산성 플라스크",
            ["All Seeing Abomination"] = "섭리의 괴수",
            ["Apron"] = "앞치마",
            ["Arc Welder"] = "아크 용접기",
            ["Bad Taste"] = "질나쁜 취향",
            ["Beetle 23"] = "Beetle 23",
            ["Bioelectric Bat"] = "생체전기 박쥐",
            ["Bunch of keys"] = "열쇠 한 무더기",
            ["Clarity"] = "명료함",
            ["Cookie Cat"] = "고양이 쿠키",
            ["Excrement"] = "배설물",
            ["Finger"] = "손가락",
            ["Glowsticks"] = "형광봉",
            ["Goo Gift"] = "점액 선물",
            ["Gunk Feast"] = "오물 만찬",
            ["Hackerbot"] = "해커봇",
            ["Heavy Saw"] = "무거운 톱",
            ["Helping Hand"] = "도움의 손길",
            ["Ladle"] = "국자",
            ["Loyal Rat"] = "충성스러운 쥐",
            ["Matchstick"] = "성냥",
            ["Medical Report"] = "의료 보고서",
            ["Penlight"] = "펜라이트",
            ["Ram"] = "들이받기",
            ["Rusted Hammer"] = "녹슨 망치",
            ["Screwdriver"] = "드라이버",
            ["Shroom Experiment"] = "버섯 실험",
            ["Slop"] = "밥찌꺼기",
            ["Soylent Green"] = "소일렌트 그린",
            ["Spore Lung"] = "포자 허파",
            ["Spores"] = "포자",
            ["Strange Key"] = "이상한 열쇠",
            ["Terminal"] = "터미널",
            ["Threat Scan"] = "위협 스캔",
            ["Toad Experiment"] = "두꺼비 실험",
            ["Vile Buffet"] = "역겨운 뷔페",
            ["Whack"] = "후려치기",
            ["Universal Damage do-er"] = "보편적 피해 카드",
            ["Box o' Blitz"] = "블릿츠 박스",
            ["Inventor's Pick"] = "발명가의 열쇠",
            ["Napkin"] = "냅킨",
            ["Clunky Blowtorch"] = "투박한 토치",
            ["Syringe"] = "주사기",
            ["Keen Eye"] = "예리한 눈",
            ["Generous"] = "너그러움",
            ["Always Looking"] = "늘 지켜보는 눈",
            ["Monster Blood"] = "괴물의 피",
            ["Rope"] = "밧줄",
            ["Loaf of Bread"] = "빵 한 덩이",
            ["Touched By Gloom"] = "어둠에 물든",
            ["Armour of Scraps"] = "고철 갑옷",
            ["Mr Spoon"] = "미스터 스푼",
            ["Plan B"] = "플랜 B",
            ["Pinch of Salt"] = "소금 한 꼬집",
            ["Shift blame"] = "책임 전가",
            ["Vending Machine"] = "자판기",
            ["Trash"] = "쓰레기",
            ["Chemical Burn"] = "화학 화상",
            ["Toxic Burn"] = "독성 화상",
            ["Slap"] = "따귀",
            ["Hoover"] = "청소기",
            ["Juice Box"] = "주스곽",
            ["Box of Sugar Bombs"] = "설탕 덩어리 상자",
        };

        private static readonly Regex LinkRegex = new(
            @"<link(?<eq>\\?=)(?<id>[^>]+)>(?<text>.*?)</link>",
            RegexOptions.Compiled | RegexOptions.Singleline
        );

        public static string LocalizeAllLinks(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return LinkRegex.Replace(text, match =>
            {
                string eq = match.Groups["eq"].Value;
                string id = match.Groups["id"].Value;
                string inner = match.Groups["text"].Value;

                if (!TextMap.TryGetValue(inner, out var localized))
                    return match.Value;

                return $"<link{eq}{id}>{localized}</link>";
            });
        }
    }

}