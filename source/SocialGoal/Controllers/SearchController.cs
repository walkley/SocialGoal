using AutoMapper;
using SocialGoal.Model.Models;
using SocialGoal.Service;
using SocialGoal.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;


namespace SocialGoal.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly IGoalService goalService;
        private readonly IUserService userService;
        private readonly IGroupService groupService;
        private readonly IMapper mapper;

        public SearchController(IGoalService goalService, IUserService userService, IGroupService groupService, IMapper mapper)
        {
            this.goalService = goalService;
            this.userService = userService;
            this.groupService = groupService;
            this.mapper = mapper;
        }

        public ViewResult SearchAll(string searchText)
        {
            SearchViewModel searchViewModel = new SearchViewModel()
            {
                Goals = mapper.Map<IEnumerable<Goal>, IEnumerable<GoalViewModel>>(goalService.SearchGoal(searchText)),
                Users = userService.SearchUser(searchText),
                Groups = mapper.Map<IEnumerable<Group>, IEnumerable<GroupViewModel>>(groupService.SearchGroup(searchText)),
                SearchText = searchText
            };
            ViewBag.searchtext = searchText;
            return View("SearchResult", searchViewModel);
        }

    }
}
