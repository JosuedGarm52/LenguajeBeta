create database Analizador;
use Analizador;
drop table compilador;
truncate table compilador;
create table compilador(
	ESTSIM smallint primary key,
	_65 smallint,
    _66 smallint,
    _67 smallint,
    _68 smallint,
    _69 smallint,
    _70 smallint,
_71 smallint,
_72 smallint,
_73 smallint,
_74 smallint,
_75 smallint,
_76 smallint,
_77 smallint,
_78 smallint,
_79 smallint,

_80 smallint,
_81 smallint,
_82 smallint,
_83 smallint,
_84 smallint,
_85 smallint,
_86 smallint,
_87 smallint,
_88 smallint,
_89 smallint,
_90 smallint,
_97 smallint,
_98 smallint,
_99 smallint,

_100 smallint,
_101 smallint,
_102 smallint,
_103 smallint,
_104 smallint,
_105 smallint,
_106 smallint,
_107 smallint,
_108 smallint,
_109 smallint,

_110 smallint,
_111 smallint,
_112 smallint,
_113 smallint,
_114 smallint,
_115 smallint,
_116 smallint,
_117 smallint,
_118 smallint,
_119 smallint,

_120 smallint,
_121 smallint,
_122 smallint,

_48 smallint,
_49 smallint,
_50 smallint,
_51 smallint,
_52 smallint,
_53 smallint,
_54 smallint,
_55 smallint,
_56 smallint,
_57 smallint,

_43 smallint, 
_45 smallint,
_47 smallint,
_42 smallint,
_60 smallint,
_62 smallint,
_95 smallint,
_40 smallint,
_41 smallint,
_123 smallint,
_125 smallint,
_91 smallint, 
_93 smallint,
_58 smallint,
_46 smallint,
_61 smallint,
_39 smallint,
_34 smallint,
_44 smallint,
_32 smallint,
_124 smallint,
_94 smallint,

FDC varchar(30),
CAT varchar(5)
);

alter table compilador
add _90 smallint;



alter table compilador

add _100 smallint,
add _101 smallint,
add _102 smallint,
add _103 smallint,
add _104 smallint,
add _105 smallint,
add _106 smallint,
add _107 smallint,
add _108 smallint,
add _109 smallint;

select * from compilador;