using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class FormaFreteService : ServiceBase<FORMA_FRETE>, IFormaFreteService
    {
        private readonly IFormaFreteRepository _baseRepository;
        protected Db_PrecificacaoEntities Db = new Db_PrecificacaoEntities();

        public FormaFreteService(IFormaFreteRepository baseRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public List<FORMA_FRETE> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }
    }
}
