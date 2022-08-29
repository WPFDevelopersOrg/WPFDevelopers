using System.Collections.ObjectModel;

namespace WPFDevelopers.Sample.Models
{
    public class HospitalList : ObservableCollection<Hospital>
    {
        public HospitalList()
        {
            var hospitals = new string[] { "No. 189, Grove St, Los Angeles", "No. 3669, Grove St, Los Angeles" };
            var names = new string[] { "Doctor Fang", "Judge Qu" };
            var images = new string[] 
                { "https://pic2.zhimg.com/80/v2-0711e97955adc9be9fbcff67e1007535_720w.jpg",
                  //"https://pic2.zhimg.com/80/v2-5b7f84c63075ba9771f6e6dc29a54615_720w.jpg",
                  "https://pic3.zhimg.com/80/v2-a3d6d8832090520e7ed6c748a8698e4e_720w.jpg",
                  "https://pic3.zhimg.com/80/v2-de7554ac9667a59255fe002bb8753ab6_720w.jpg"
                };
            var state = 0;
            for (var i = 1; i < 10000; i++)
            {
                Add(new Hospital { Id = $"9999{i}", DoctorName = i % 2 == 0 ? names[0]:names[1], HospitalName = i % 2 == 0 ? hospitals[0] : hospitals[1] ,State = state ,UserImage = images[state] });
                state++;
                if (state > 2)
                    state = 0;
            }
        }
    }

    public class Hospital
    {
        public string Id { get; set; }
        public string DoctorName { get; set; }
        public string HospitalName { get; set; }
        public string UserImage { get; set; }
        public int State { get; set; }
    }
}