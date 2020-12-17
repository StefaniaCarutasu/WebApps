﻿using MicroSocialPlatform.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace MicroSocialPlatform.Controllers
{
    public class ProfilesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Profiles

        [Authorize(Roles = "User,Admin")]
        public ActionResult Index()
        {
            string id = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Find(id);
            ViewBag.Posts = (from post in db.Posts
                             where post.UserId == id
                             select post).ToList();
            ViewBag.ProfileDescription = user.ProfileDescription;
            ViewBag.UserId = id;
            ViewBag.User = user;
            var fr = (from friend in db.Friends
                      where friend.User2_Id == id && friend.Accepted == false
                      select friend);
            ViewBag.FriendshipRequests = fr;
            ViewBag.FrReqCount = fr.Count();
            var isFriend = (from friend in db.Friends
                            where ((friend.User2_Id == id) || (friend.User1_Id == id)) && friend.Accepted == true
                            select friend).ToList();
            ViewBag.UserFriends = isFriend;
            return View();
        }
        [Authorize(Roles = "User,Admin")]
        public ActionResult Show(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            ViewBag.Posts = (from post in db.Posts
                            where post.UserId == id
                            select post).ToList();
            ViewBag.Profile = from profile in db.Profiles
                              where profile.UserId == id
                              select profile;
            ViewBag.User = user;
            ViewBag.CurrentUser = db.Users.Find(User.Identity.GetUserId());
           string currentId = User.Identity.GetUserId();
            ViewBag.Friends = db.Friends;
            ViewBag.FriendCount = db.Friends.Count();
            var isFriend = (from friend in db.Friends
                           where ((friend.User2_Id == id)|| (friend.User1_Id == id)) && friend.Accepted == true
                           select friend).ToList();
            ViewBag.UserFriends = isFriend;
            ViewBag.isAdmin = false;
            if (User.IsInRole("Admin"))
            {
                ViewBag.isAdmin = true;
            }
            

            return View(user);
        }

        [Authorize(Roles = "User,Admin")]
        public ActionResult Edit()
        {
            string id = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Find(id);
            return View(user);
        }

        [HttpPut]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Edit(ApplicationUser requestUser)
        {
            string id = User.Identity.GetUserId();
            try
            {
                ApplicationUser user = db.Users.Find(id);
                if (TryUpdateModel(user))
                {
                    user.UserName = requestUser.UserName;
                    user.BirthDate = requestUser.BirthDate;
                    user.PhoneNumber = requestUser.PhoneNumber;
                    user.ProfileDescription = requestUser.ProfileDescription;
                    db.SaveChanges();
                    TempData["message"] = "Profilul a fost editat cu succes";
                    return RedirectToAction("Index");

                }
                return View(requestUser);
            }
            catch (Exception e)
            {
                return View(requestUser);
            }
        }
    }
}