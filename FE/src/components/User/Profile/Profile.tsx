import { useState } from 'react'
import './Profile.css'
import { MdEmail } from "react-icons/md";
import { FaFacebook, FaTwitter, FaInstagram } from "react-icons/fa";
import avatarImg from '../../../assets/avatar.jpg'
import { FiEdit2 } from 'react-icons/fi';

interface ProfileData {
  name: string;
  title: string;
  location: string;
  followers: number;
  following: number;
  email: string;
  languages: string[];
  nickname: string;
  joinDate: string;
  workHistory: string[];
  education: string;
  about: string;
}

interface LikedPost {
  id: string;
  title: string;
  image: string;
  author: string;
  likes: number;
  datePosted: string;
}

interface WorkPost {
  id: string;
  title: string;
  image: string;
  description: string;
  category: string;
  views: number;
  likes: number;
  datePosted: string;
}

export default function Profile() {
  const [profile, setProfile] = useState<ProfileData>({
    name: "Rick Roll",
    title: "UI/UX Designer",
    location: "Ha Noi, Viet Nam",
    followers: 852,
    following: 156,
    email: "duynmse173649@fpt.edu.vn",
    languages: ["Vietnamese", "English"],
    nickname: "FreelanceBie",
    joinDate: "2 months ago, on 08/26/2024",
    workHistory: ["Petrovna.ai", "Shop.inc", "P.i"],
    education: "Standford University",
    about: "Hi, my name is John there! I'm the Co-founder and Head of Design at BB agency. Designer at heart. Head of Design might be an overstatement, but as with many 20 people agencies I need to wear many different hats. I manage creative teams and set up processes that allow our collaborators and clients to achieve growth, scalability, and progress."
  });

  const [likedPosts, setLikedPosts] = useState<LikedPost[]>([
    {
      id: '1',
      title: 'Modern Dashboard Design Trends',
      image: '/src/assets/Post Like.jpg',
      author: 'Jane Cooper',
      likes: 234,
      datePosted: '2024-03-15'
    },
    {
      id: '2',
      title: 'Mobile App UI Collection',
      image: '/src/assets/Post Like.jpg',
      author: 'Robert Fox',
      likes: 189,
      datePosted: '2024-03-14'
    },
    {
      id: '3',
      title: 'E-commerce Website Redesign',
      image: '/src/assets/Post Like.jpg',
      author: 'Wade Warren',
      likes: 456,
      datePosted: '2024-03-13'
    },
    {
      id: '4',
      title: 'Financial App Interface',
      image: '/src/assets/Post Like.jpg',
      author: 'Esther Howard',
      likes: 321,
      datePosted: '2024-03-12'
    },
    {
      id: '5',
      title: 'Travel App Design',
      image: '/src/assets/Post Like.jpg',
      author: 'Jenny Wilson',
      likes: 287,
      datePosted: '2024-03-11'
    },
    {
      id: '6',
      title: 'Social Media Dashboard',
      image: '/src/assets/Post Like.jpg',
      author: 'Guy Hawkins',
      likes: 198,
      datePosted: '2024-03-10'
    },
    {
      id: '7',
      title: 'NFT Marketplace Design',
      image: '/src/assets/Post Like.jpg',
      author: 'Brooklyn Simmons',
      likes: 432,
      datePosted: '2024-03-09'
    },
    {
      id: '8',
      title: 'Smart Home App Interface',
      image: '/src/assets/Post Like.jpg',
      author: 'Cameron Williamson',
      likes: 167,
      datePosted: '2024-03-08'
    },
    {
      id: '9',
      title: 'Fitness Tracking Dashboard',
      image: '/src/assets/Post Like.jpg',
      author: 'Leslie Alexander',
      likes: 298,
      datePosted: '2024-03-07'
    },
    {
      id: '10',
      title: 'Food Delivery App Design',
      image: '/src/assets/Post Like.jpg',
      author: 'Kristin Watson',
      likes: 345,
      datePosted: '2024-03-06'
    }
  ]);

  const [workPosts] = useState<WorkPost[]>([
    {
      id: '1',
      title: 'Mobile Banking App Design', 
      image: '/src/assets/Post Like.jpg',
      description: 'A modern banking app designed with user experience in mind',
      category: 'Mobile App',
      views: 1234,
      likes: 423,
      datePosted: '2024-03-15'
    },
    {
      id: '2',
      title: 'E-commerce Website Redesign',
      image: '/src/assets/Post Like.jpg',
      description: 'Complete redesign of an e-commerce platform',
      category: 'Web Design',
      views: 892,
      likes: 345,
      datePosted: '2024-03-10'
    },
    {
      id: '3',
      title: 'Food Delivery App UI Kit',
      image: '/src/assets/Post Like.jpg',
      description: 'UI kit for food delivery applications',
      category: 'UI Kit',
      views: 2156,
      likes: 567,
      datePosted: '2024-03-05'
    },
    {
      id: '4',
      title: 'Social Media Dashboard',
      image: '/src/assets/Post Like.jpg',
      description: 'Analytics dashboard for social media management',
      category: 'Dashboard',
      views: 1567,
      likes: 289,
      datePosted: '2024-02-28'
    },
    {
      id: '5',
      title: 'Travel App Interface',
      image: '/src/assets/Post Like.jpg',
      description: 'Modern travel application interface design',
      category: 'Mobile App',
      views: 1890,
      likes: 456,
      datePosted: '2024-02-20'
    }
  ]);

  const [activeTab, setActiveTab] = useState<string>('work');
  const [isEditing, setIsEditing] = useState(false);
  const [editValue, setEditValue] = useState('');
  const [editField, setEditField] = useState('');
  const [isEditPopupOpen, setIsEditPopupOpen] = useState(false);

  const handleTabClick = (tab: string) => (e: React.MouseEvent) => {
    e.preventDefault();
    setActiveTab(tab);
  };

  const handleEditClick = (field: string, value: string) => {
    setEditField(field);
    setEditValue(value);
    setIsEditPopupOpen(true);
  };

  const handleChange = (field: keyof ProfileData, value: string) => {
    setProfile(prev => {
      const newProfile = { ...prev };
      
      // Handle arrays
      if (field === 'languages' || field === 'workHistory') {
        newProfile[field] = value.split(',').map(item => item.trim());
      } else {
        newProfile[field] = value;
      }
      
      return newProfile;
    });
  };

  const handleLikePost = (postId: string) => {
    setLikedPosts(prev => {
      const post = prev.find(p => p.id === postId);
      if (post) {
        return prev.map(p => 
          p.id === postId 
            ? { ...p, likes: p.likes + 1 }
            : p
        );
      }
      return prev;
    });
  };

  const handleRemoveLikedPost = (postId: string) => {
    if (window.confirm('Are you sure you want to remove this post from liked posts?')) {
      setLikedPosts(prev => prev.filter(p => p.id !== postId));
    }
  };

  const handleSave = () => {
    setProfile(prev => ({
      ...prev,
      [editField.toLowerCase()]: editValue
    }));
    setIsEditPopupOpen(false);
  };

  return (
    <div className="profile-page">
      <header className="profile-header">
        <div className="cover-image"></div>
        <div className="profile-info">
          <div className="avatar">
            <img src={avatarImg} alt="Profile Avatar" />
          </div>
          <div className="main-info">
            <div className="name-section">
              <h1>{profile.name}</h1>
              <button className="like-btn">♡</button>
              <button className="edit-btn">Edit Profile</button>
            </div>
            <p className="title">{profile.title}</p>
            <p className="location">{profile.location}</p>
            <div className="social-links">
              <a href="#" className="social-icon">
                <MdEmail size={24} />
              </a>
              <a href="#" className="social-icon">
                <FaFacebook size={24} />
              </a>
              <a href="#" className="social-icon">
                <FaTwitter size={24} />
              </a>
              <a href="#" className="social-icon">
                <FaInstagram size={24} />
              </a>
            </div>
          </div>
          <div className="stats">
            <div className="stat-item">
              <span className="number">{profile.followers}</span>
              <span className="label">Followers</span>
            </div>
            <div className="stat-item">
              <span className="number">{profile.following}</span>
              <span className="label">Following</span>
            </div>
          </div>
        </div>
      </header>

      <div className="profile-content">
        <div className="main-content">
          <nav className="profile-nav">
            <a 
              href="#" 
              className={activeTab === 'work' ? 'active' : ''}
              onClick={handleTabClick('work')}
            >
              Work
            </a>
            <a 
              href="#" 
              className={activeTab === 'collection' ? 'active' : ''}
              onClick={handleTabClick('collection')}
            >
              Collection
            </a>
            <a 
              href="#" 
              className={activeTab === 'liked' ? 'active' : ''}
              onClick={handleTabClick('liked')}
            >
              Liked Posts
            </a>
            <a 
              href="#" 
              className={activeTab === 'comments' ? 'active' : ''}
              onClick={handleTabClick('comments')}
            >
              Comments
            </a>
            <a 
              href="#" 
              className={activeTab === 'about' ? 'active' : ''}
              onClick={handleTabClick('about')}
            >
              About
            </a>
          </nav>

          {activeTab === 'work' && (
            <section className="work-posts">
              <div className="posts-grid">
                {workPosts.map(post => (
                  <div key={post.id} className="post-card work-card">
                    <div className="post-image">
                      <img src={post.image} alt={post.title} />
                    </div>
                    <div className="post-info">
                      <span className="category">{post.category}</span>
                      <h3>{post.title}</h3>
                      <p className="description">{post.description}</p>
                      <div className="post-meta">
                        <span className="views">👁 {post.views}</span>
                        <span className="likes">♥ {post.likes}</span>
                        <span className="date">{new Date(post.datePosted).toLocaleDateString()}</span>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </section>
          )}

          {activeTab === 'liked' && (
            <section className="liked-posts">
              <div className="posts-grid">
                {likedPosts.map(post => (
                  <div key={post.id} className="post-card">
                    <div className="post-image">
                      <img src={post.image} alt={post.title} />
                    </div>
                    <div className="post-info">
                      <h3>{post.title}</h3>
                      <div className="post-meta">
                        <span className="author">by {post.author}</span>
                        <div className="actions">
                          <button 
                            className="like-button"
                            onClick={() => handleLikePost(post.id)}
                          >
                            ♥ {post.likes}
                          </button>
                          <button 
                            className="remove-button"
                            onClick={() => handleRemoveLikedPost(post.id)}
                          >
                            ✕
                          </button>
                        </div>
                        <span className="date">
                          {new Date(post.datePosted).toLocaleDateString()}
                        </span>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </section>
          )}

          {activeTab === 'collection' && (
            <section className="collection">
              <h2>My Collections</h2>
              <p>Collections coming soon...</p>
            </section>
          )}

          {activeTab === 'comments' && (
            <section className="comments">
              <h2>My Comments</h2>
              <p>Comments coming soon...</p>
            </section>
          )}

          {activeTab === 'about' && (
            <section className="about-section">
              <h2>About Me</h2>
              <p>{profile.about}</p>
            </section>
          )}
        </div>

        <aside className="additional-details">
          <div className="section-header">
            <h3>Additional Details</h3>
          </div>
          
          <div className="details-list">
            <div className="detail-item">
              <span className="label">Email</span>
              <span className="value">{profile.email}</span>
              <button 
                className="edit-icon"
                onClick={() => handleEditClick('Email', profile.email)}
              >
                <FiEdit2 />
              </button>
            </div>

            <div className="detail-item">
              <span className="label">Languages</span>
              <span className="value">{profile.languages.join(', ')}</span>
              <button 
                className="edit-icon"
                onClick={() => handleEditClick('Languages', profile.languages.join(', '))}
              >
                <FiEdit2 />
              </button>
            </div>

            <div className="detail-item">
              <span className="label">Nickname</span>
              <span className="value">{profile.nickname}</span>
              <button 
                className="edit-icon"
                onClick={() => handleEditClick('Nickname', profile.nickname)}
              >
                <FiEdit2 />
              </button>
            </div>

            <div className="detail-item">
              <span className="label">Work History</span>
              <span className="value">{profile.workHistory.join(', ')}</span>
              <button 
                className="edit-icon"
                onClick={() => handleEditClick('Work History', profile.workHistory.join(', '))}
              >
                <FiEdit2 />
              </button>
            </div>

            <div className="detail-item">
              <span className="label">Education</span>
              <span className="value">{profile.education}</span>
              <button 
                className="edit-icon"
                onClick={() => handleEditClick('Education', profile.education)}
              >
                <FiEdit2 />
              </button>
            </div>
          </div>
        </aside>
      </div>

      {isEditPopupOpen && (
        <div className="edit-popup-overlay">
          <div className="edit-popup">
            <div className="edit-popup-header">
              <h3>Edit {editField}</h3>
              <button 
                className="close-btn"
                onClick={() => setIsEditPopupOpen(false)}
              >
                ×
              </button>
            </div>
            <div className="edit-popup-content">
              <input
                type="text"
                value={editValue}
                onChange={(e) => setEditValue(e.target.value)}
                className="edit-popup-input"
                placeholder={`Enter ${editField.toLowerCase()}`}
              />
            </div>
            <div className="edit-popup-actions">
              <button 
                className="cancel-btn"
                onClick={() => setIsEditPopupOpen(false)}
              >
                Cancel
              </button>
              <button 
                className="save-btn"
                onClick={handleSave}
              >
                Save Changes
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}