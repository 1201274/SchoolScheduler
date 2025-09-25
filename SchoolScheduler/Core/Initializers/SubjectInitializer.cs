using System.Collections.Generic;
using SchoolScheduler.Models;

namespace SchoolScheduler.Core.Initializers
{
    using ClassInfo = (int TimeSlotsPerWeek, List<string> Teachers, RoomType RoomType);
    public class SubjectInitializer
    {
        public List<Subject> Initialize()
        {
            var subjects = new List<Subject>();
            subjects.AddRange(BachelorsInformaticsEngineering());
            return subjects;
        }

        private List<Subject> BachelorsInformaticsEngineering()
        {
            var subjects = new List<Subject>();
            subjects.AddRange(BachelorsInformaticsEngineeringYear1());
            subjects.AddRange(BachelorsInformaticsEngineeringYear2());
            subjects.AddRange(BachelorsInformaticsEngineeringYear3());
            return subjects;
        }

        private List<Subject> BachelorsInformaticsEngineeringYear1()
        {
            var course = new Course
            {
                Name = "Bachelor's in Informatics",
                Year = 1,
                Department = Department.INFORMATICS
            };

            return new List<Subject>
            {
                new Subject("SBJ_A", 25 * 14, Department.INFORMATICS, course, new Dictionary<ClassTypeEnum, ClassInfo>
                {
                    { ClassTypeEnum.T, (TimeSlotsPerWeek: 2, Teachers: new List<string> { "Prof. AA" }, RoomType: RoomType.AUDITORIUM) },
                    { ClassTypeEnum.TP, (TimeSlotsPerWeek: 2, Teachers: new List<string> { "Prof. AB", "Prof. AC", "Prof. AD", "Prof. AE", "Prof. AF" }, RoomType: RoomType.AUDITORIUM | RoomType.REGULAR) },
                    { ClassTypeEnum.PL, (TimeSlotsPerWeek: 6, Teachers: new List<string> { "Prof. AB", "Prof. AG", "Prof. AH", "Prof. AI", "Prof. AJ", "Prof. AD", "Prof. AE", "Prof. AF", "Prof. AK" }, RoomType: RoomType.REGULAR | RoomType.LAB) }
                }),

                new Subject("SBJ_B", 25 * 16, Department.INFORMATICS, course, new Dictionary<ClassTypeEnum, ClassInfo>
                {
                    { ClassTypeEnum.T, (TimeSlotsPerWeek: 2, Teachers: new List<string> { "Prof. AL" }, RoomType: RoomType.AUDITORIUM) },
                    { ClassTypeEnum.TP, (TimeSlotsPerWeek: 2, Teachers: new List<string> { "Prof. AL", "Prof. AM", "Prof. AN", "Prof. AO", "Prof. AP", "Prof. AQ" }, RoomType: RoomType.AUDITORIUM | RoomType.REGULAR) },
                    { ClassTypeEnum.PL, (TimeSlotsPerWeek: 6, Teachers: new List<string> { "Prof. AL", "Prof. AM", "Prof. AR", "Prof. AS", "Prof. AN", "Prof. AT", "Prof. AU", "Prof. AO", "Prof. AV", "Prof. AP", "Prof. AQ", "Prof. AW", "Prof. AX" }, RoomType: RoomType.REGULAR | RoomType.LAB) }
                }),

                new Subject("SBJ_C", 25 * 16, Department.INFORMATICS, course, new Dictionary<ClassTypeEnum, ClassInfo>
                {
                    { ClassTypeEnum.T, (TimeSlotsPerWeek: 2, Teachers: new List<string> { "Prof. AY" }, RoomType: RoomType.AUDITORIUM) },
                    { ClassTypeEnum.TP, (TimeSlotsPerWeek: 6, Teachers: new List<string> { "Prof. AZ", "Prof. BA", "Prof. BB", "Prof. BC" }, RoomType: RoomType.AUDITORIUM | RoomType.REGULAR) }
                }),

                new Subject("SBJ_D", 25 * 16, Department.INFORMATICS, course, new Dictionary<ClassTypeEnum, ClassInfo>
                {
                    { ClassTypeEnum.T, (TimeSlotsPerWeek: 2, Teachers: new List<string> { "Prof. BD" }, RoomType: RoomType.AUDITORIUM) },
                    { ClassTypeEnum.TP, (TimeSlotsPerWeek: 3, Teachers: new List<string> { "Prof. BE", "Prof. BF", "Prof. BD", "Prof. BG", "Prof. BH", "Prof. BI", "Prof. BJ" }, RoomType: RoomType.AUDITORIUM | RoomType.REGULAR) },
                    { ClassTypeEnum.PL, (TimeSlotsPerWeek: 3, Teachers: new List<string> { "Prof. BE", "Prof. BF", "Prof. BG", "Prof. BH", "Prof. BI", "Prof. BJ" }, RoomType: RoomType.REGULAR | RoomType.LAB) }
                }),

                new Subject("SBJ_E", 25 * 16, Department.INFORMATICS, course, new Dictionary<ClassTypeEnum, ClassInfo>
                {
                    { ClassTypeEnum.TP, (TimeSlotsPerWeek: 4, Teachers: new List<string> { "Prof. AL", "Prof. BK", "Prof. BL" }, RoomType: RoomType.AUDITORIUM | RoomType.REGULAR) },
                }),
            };
        }

        private List<Subject> BachelorsInformaticsEngineeringYear2()
        {
            var course = new Course
            {
                Name = "Bachelor's in Informatics",
                Year = 2,
                Department = Department.INFORMATICS
            };

            return new List<Subject>
            {
                new Subject("SBJ_F", 25 * 14, Department.INFORMATICS, course, new Dictionary<ClassTypeEnum, ClassInfo>
                {
                    { ClassTypeEnum.T, (TimeSlotsPerWeek: 2, Teachers: new List<string> { "Prof. BM" }, RoomType: RoomType.AUDITORIUM) },
                    { ClassTypeEnum.TP, (TimeSlotsPerWeek: 2, Teachers: new List<string> { "Prof. BN", "Prof. BM", "Prof. BO", "Prof. BP", "Prof. BQ", "Prof. BR", "Prof. BS", "Prof. BT" }, RoomType: RoomType.AUDITORIUM | RoomType.REGULAR) },
                    { ClassTypeEnum.PL, (TimeSlotsPerWeek: 6, Teachers: new List<string> { "Prof. BN", "Prof. BO", "Prof. BP", "Prof. BU", "Prof. BQ", "Prof. BR", "Prof. BS", "Prof. BV", "Prof. BT" }, RoomType: RoomType.REGULAR | RoomType.LAB) }
                }),

                new Subject("SBJ_G", 25 * 14, Department.INFORMATICS, course, new Dictionary<ClassTypeEnum, ClassInfo>
                {
                    { ClassTypeEnum.T, (TimeSlotsPerWeek: 2, Teachers: new List<string> { "Prof. BW" }, RoomType: RoomType.AUDITORIUM) },
                    { ClassTypeEnum.TP, (TimeSlotsPerWeek: 2, Teachers: new List<string> { "Prof. BW", "Prof. BX", "Prof. BY", "Prof. BZ", "Prof. CA", "Prof. CB" }, RoomType: RoomType.AUDITORIUM | RoomType.REGULAR) },
                    { ClassTypeEnum.PL, (TimeSlotsPerWeek: 6, Teachers: new List<string> { "Prof. BW", "Prof. AR", "Prof. BX", "Prof. CC", "Prof. BY", "Prof. BZ", "Prof. CD", "Prof. CA", "Prof. CB" }, RoomType: RoomType.REGULAR | RoomType.LAB) }
                }),

                new Subject("SBJ_H", 25 * 14, Department.INFORMATICS, course, new Dictionary<ClassTypeEnum, ClassInfo>
                {
                    { ClassTypeEnum.T, (TimeSlotsPerWeek: 2, Teachers: new List<string> { "Prof. CE" }, RoomType: RoomType.AUDITORIUM) },
                    { ClassTypeEnum.TP, (TimeSlotsPerWeek: 2, Teachers: new List<string> { "Prof. CF", "Prof. CG", "Prof. CH", "Prof. CE", "Prof. CI", "Prof. CJ", "Prof. CK" }, RoomType: RoomType.AUDITORIUM | RoomType.REGULAR) },
                    { ClassTypeEnum.PL, (TimeSlotsPerWeek: 6, Teachers: new List<string> { "Prof. CF", "Prof. CG", "Prof. CH", "Prof. CE", "Prof. CI", "Prof. CJ", "Prof. CK", "Prof. BS", "Prof. CL" }, RoomType: RoomType.REGULAR | RoomType.LAB) }
                }),

                new Subject("SBJ_I", 25 * 14, Department.INFORMATICS, course, new Dictionary<ClassTypeEnum, ClassInfo>
                {
                    { ClassTypeEnum.T, (TimeSlotsPerWeek: 2, Teachers: new List<string> { "Prof. CM" }, RoomType: RoomType.AUDITORIUM) },
                    { ClassTypeEnum.TP, (TimeSlotsPerWeek: 2, Teachers: new List<string> { "Prof. CN", "Prof. CO", "Prof. CP", "Prof. CQ", "Prof. CR" }, RoomType: RoomType.AUDITORIUM | RoomType.REGULAR) },
                    { ClassTypeEnum.PL, (TimeSlotsPerWeek: 6, Teachers: new List<string> { "Prof. CS", "Prof. CN", "Prof. CT", "Prof. CO", "Prof. CU", "Prof. CP", "Prof. CV", "Prof. CR" }, RoomType: RoomType.REGULAR | RoomType.LAB) }
                }),

                new Subject("SBJ_J", 25 * 14, Department.INFORMATICS, course, new Dictionary<ClassTypeEnum, ClassInfo>
                {
                    { ClassTypeEnum.TP, (TimeSlotsPerWeek: 3, Teachers: new List<string> { "Prof. CW", "Prof. CX", "Prof. CY" }, RoomType: RoomType.AUDITORIUM | RoomType.REGULAR) },
                    { ClassTypeEnum.PL, (TimeSlotsPerWeek: 2, Teachers: new List<string> { "Prof. BW", "Prof. BK", "Prof. BL", "Prof. CV", "Prof. CZ", "Prof. DA", "Prof. DB", "Prof. DC", "Prof. BR", "Prof. DD", "Prof. CB" }, RoomType: RoomType.REGULAR | RoomType.LAB) },
                    { ClassTypeEnum.OT, (TimeSlotsPerWeek: 3, Teachers: new List<string> { "Prof. BW", "Prof. CG", "Prof. BK", "Prof. BL", "Prof. CV", "Prof. CZ", "Prof. DA", "Prof. DB", "Prof. DC", "Prof. BR", "Prof. DD", "Prof. CB" }, RoomType: RoomType.AUDITORIUM) },
                }),
            };
        }
    

        private List<Subject> BachelorsInformaticsEngineeringYear3()
        {
            var course = new Course
            {
                Name = "Bachelor's in Informatics",
                Year = 3,
                Department = Department.INFORMATICS
            };

            return new List<Subject>
            {
                new Subject("SBJ_K", 25 * 12, Department.INFORMATICS, course, new Dictionary<ClassTypeEnum, ClassInfo>
                {
                    { ClassTypeEnum.T, (TimeSlotsPerWeek: 2, Teachers: new List<string> { "Prof. BX", "Prof. DE", "Prof. DF", "Prof. CZ", "Prof. DB" }, RoomType: RoomType.AUDITORIUM) },
                    { ClassTypeEnum.PL, (TimeSlotsPerWeek: 4, Teachers: new List<string> { "Prof. DG", "Prof. DH", "Prof. AO", "Prof. DI", "Prof. CZ", "Prof. DB", "Prof. DD" }, RoomType: RoomType.REGULAR | RoomType.LAB) }
                }),

                new Subject("SBJ_L", 25 * 12, Department.INFORMATICS, course, new Dictionary<ClassTypeEnum, ClassInfo>
                {
                    { ClassTypeEnum.T, (TimeSlotsPerWeek: 2, Teachers: new List<string> { "Prof. BM", "Prof. DJ" }, RoomType: RoomType.AUDITORIUM) },
                    { ClassTypeEnum.TP, (TimeSlotsPerWeek: 4, Teachers: new List<string> { "Prof. DK", "Prof. DJ", "Prof. DL", "Prof. DM" }, RoomType: RoomType.AUDITORIUM | RoomType.REGULAR) }
                }),

                new Subject("SBJ_M", 25 * 12, Department.INFORMATICS, course, new Dictionary<ClassTypeEnum, ClassInfo>
                {
                    { ClassTypeEnum.T, (TimeSlotsPerWeek: 2, Teachers: new List<string> { "Prof. CW", "Prof. DN" }, RoomType: RoomType.AUDITORIUM) },
                    { ClassTypeEnum.PL, (TimeSlotsPerWeek: 4, Teachers: new List<string> { "Prof. CW", "Prof. DN", "Prof. CX", "Prof. DO", "Prof. DP", "Prof. DQ", "Prof. DR" }, RoomType: RoomType.REGULAR | RoomType.LAB) }
                }),
            };
        }
    }
}
