﻿using System.Linq;
using System.Threading.Tasks;
using BLL.Helpers.Queries.Interfaces;
using BLL.Helpers.UserUpdating.Interfaces;
using BLL.Models;
using DAL.DBModels;
using DAL.Repositories.Interfaces;
using Stripe;

namespace BLL.Helpers.UserUpdating
{
    public class UserModifier: IUserModifier
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserQueryCreator _userQueryCreator;

        public UserModifier(IUserRepository userRepository, IUserQueryCreator userQueryCreator)
        {
            _userRepository = userRepository;
            _userQueryCreator = userQueryCreator;
        }

        public async Task<UserDTO> GetOrCreateUser(PaymentModel payment)
        {
            var query = await _userQueryCreator.GetUser(payment.Email);
            return  (await _userRepository.GetUser(query)).LastOrDefault() ?? await CreateUser(payment);
        }

        private async Task<UserDTO> CreateUser(PaymentModel payment)
        {
            var customerOptions = new CustomerCreateOptions
            {
                Source = payment.CardToken,
                Email = payment.Email,
            };
            var customerService = new CustomerService();
            var customer = customerService.Create(customerOptions);

            var user = new UserDTO()
            {
                Email = payment.Email,
                ExternalId = customer.Id,
            };
            if (payment.SaveCard) return await _userRepository.CreateUser(user);
            else return user;
        }
    }
}