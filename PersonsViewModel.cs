using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WpfUI;

namespace WPFPages.ViewModels
{
	public class PersonsViewModel : ObservableCollection<Person>
     {
          public event PropertyChangedEventHandler PropertyChanged;

          //public string NameToDisplay { get; set; }
          public List<Person> ListOfPersons { get; set; }
          public ObservableCollection<Person> People = new ObservableCollection<Person>();

          public MainViewModel()
          {
               //NameToDisplay = "Hello Ian & Olwen";
               //Assign the List of Persons to an ObservableCollecton<Person> named People
               People = GetListOfPersons();
       
          }

          private ObservableCollection<Person> GetListOfPersons ( )
          {
                  Person fabianPerson = GetPerson("Fabian", 29);
                  Person evePerson = GetPerson("Eve", 100);
                  Person jPerson = GetPerson("John", 60);
                  Person kPerson = GetPerson("Kinder", 88);
                  Person yPerson = GetPerson("Yvonne", 23);
                  People.Add(fabianPerson);
                  People.Add(evePerson);
                  People.Add(jPerson);
                  People.Add(kPerson);
                  People.Add(yPerson);
                  fabianPerson = GetPerson ( "Ian" , 77 );
                  evePerson = GetPerson ( "Olwen" , 72 );
                  jPerson = GetPerson ( "Chris" , 65 );
                  kPerson = GetPerson ( "Juliana" , 18 );
                  yPerson = GetPerson ( "Yvette" , 27 );
                  People .Add ( fabianPerson );
                  People .Add ( evePerson );
                  People .Add ( jPerson );
                  People .Add ( kPerson );
                  People .Add ( yPerson );
                  return People;
          }

          private Person GetPersonbyName(string name)
          {
			foreach ( var item in People )
			{
                        if(item.)
			}
               return new Person();
          }
          private void OnPropertyChanged(string propertyName)
          {
               PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
          }
          private void RaisePropertyChanged(string propName)
          {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
		}
     }
}
