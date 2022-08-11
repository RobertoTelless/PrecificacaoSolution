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
    public class PessoaExternaService : ServiceBase<PESSOA_EXTERNA>, IPessoaExternaService
    {
        private readonly IPessoaExternaRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ICargoRepository _carRepository;
        protected Db_PrecificacaoEntities Db = new Db_PrecificacaoEntities();

        public PessoaExternaService(IPessoaExternaRepository baseRepository, ILogRepository logRepository, ICargoRepository carRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _carRepository = carRepository;

        }

        public PESSOA_EXTERNA GetItemById(Int32 id)
        {
            PESSOA_EXTERNA item = _baseRepository.GetItemById(id);
            return item;
        }

        public PESSOA_EXTERNA GetByEmail(String email, Int32 idAss)
        {
            PESSOA_EXTERNA item = _baseRepository.GetByEmail(email, idAss);
            return item;
        }

        public List<PESSOA_EXTERNA> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<PESSOA_EXTERNA> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public PESSOA_EXTERNA CheckExist(PESSOA_EXTERNA conta, Int32 idAss)
        {
            PESSOA_EXTERNA item = _baseRepository.CheckExist(conta, idAss);
            return item;
        }

        public List<PESSOA_EXTERNA> ExecuteFilter(Int32? cargo, String nome, String cpf, String email, Int32 idAss)
        {
            List<PESSOA_EXTERNA> lista = _baseRepository.ExecuteFilter(cargo, nome, cpf, email, idAss);
            return lista;
        }

        public List<CARGO_USUARIO> GetAllCargos(Int32 idAss)
        {
            return _carRepository.GetAllItens(idAss);
        }

        public Int32 Create(PESSOA_EXTERNA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _baseRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Create(PESSOA_EXTERNA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _baseRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }


        public Int32 Edit(PESSOA_EXTERNA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PESSOA_EXTERNA obj = _baseRepository.GetById(item.PEEX_CD_ID);
                    _baseRepository.Detach(obj);
                    _logRepository.Add(log);
                    _baseRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Edit(PESSOA_EXTERNA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PESSOA_EXTERNA obj = _baseRepository.GetById(item.PEEX_CD_ID);
                    _baseRepository.Detach(obj);
                    _baseRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Delete(PESSOA_EXTERNA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _baseRepository.Remove(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}
