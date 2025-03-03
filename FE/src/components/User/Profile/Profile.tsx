import { useState, useEffect, useRef } from 'react'
import './Profile.css'
import { MdEmail } from "react-icons/md";
import { FaFacebook, FaTwitter, FaInstagram } from "react-icons/fa";
import avatarImg from '../../../assets/avatar.jpg'
import { FiEdit2 } from 'react-icons/fi';
import { ApiGateway } from '../../../services/api/ApiService';
import Quill from 'quill';
import 'quill/dist/quill.snow.css';

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
	category: string;
	views: number;
	likes: number;
	datePosted: string;
}

// Thêm interface cho validation
interface ValidationRules {
	[key: string]: {
		required?: boolean;
		minLength?: number;
		maxLength?: number;
		pattern?: RegExp;
		errorMessage?: string;
	}
}

interface EditableProfile {
	email: string;
	languages: string;
	nickname: string;
	workHistory: string;
	education: string;
}

export default function Profile() {

	const [profile, setProfile] = useState<ProfileData>({
		name: "Rick Roll",
		title: "UI/UX Designer",
		location: "Ha Noi, Viet Nam",
		followers: 852,
		following: 156,
		email: "hunglcse161248@fpt.edu.vn",
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

	const [workPosts, setWorkPosts] = useState<WorkPost[]>([]);

	const [activeTab, setActiveTab] = useState<string>('work');
	const [editValue, setEditValue] = useState('');
	const [editField, setEditField] = useState('');
	const [isEditPopupOpen, setIsEditPopupOpen] = useState(false);

	// Cập nhật state để theo dõi lỗi cho từng trường
	const [validationErrors, setValidationErrors] = useState<{ [key: string]: string }>({});

	// Định nghĩa rules cho từng trường
	const validationRules: ValidationRules = {
		email: {
			required: true,
			pattern: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
			errorMessage: 'Please enter a valid email address'
		},
		languages: {
			required: true,
			pattern: /^[a-zA-Z\s,]+$/,
			errorMessage: 'Languages should only contain letters, spaces, and commas'
		},
		nickname: {
			required: true,
			minLength: 3,
			maxLength: 20,
			pattern: /^[a-zA-Z0-9_]+$/,
			errorMessage: 'Nickname should be 3-20 characters and contain only letters, numbers, and underscores'
		},
		workHistory: {
			required: true,
			pattern: /^[a-zA-Z0-9\s,.-]+$/,
			errorMessage: 'Work history should not contain special characters'
		},
		education: {
			required: true,
			pattern: /^[a-zA-Z0-9\s,.-]+$/,
			errorMessage: 'Education should not contain special characters'
		}
	};

	// Cập nhật hàm validateField
	const validateField = (field: string, value: string): boolean => {
		const rules = validationRules[field.toLowerCase()];
		if (!rules) return true;

		setValidationErrors(prev => ({ ...prev, [field]: '' }));

		if (rules.required && !value.trim()) {
			setValidationErrors(prev => ({
				...prev,
				[field]: `${field} is required`
			}));
			return false;
		}

		if (rules.minLength && value.length < rules.minLength) {
			setValidationErrors(prev => ({
				...prev,
				[field]: `${field} must be at least ${rules.minLength} characters`
			}));
			return false;
		}

		if (rules.maxLength && value.length > rules.maxLength) {
			setValidationErrors(prev => ({
				...prev,
				[field]: `${field} must be less than ${rules.maxLength} characters`
			}));
			return false;
		}

		if (rules.pattern && !rules.pattern.test(value)) {
			setValidationErrors(prev => ({
				...prev,
				[field]: rules.errorMessage || `Invalid ${field} format`
			}));
			return false;
		}

		return true;
	};

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
				(newProfile as any)[field] = value;
			}

			return newProfile;
		});
	};

	const hanldeAddPostClick = () => {
		window.location.href = '/shot/edit'
	}

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

	// Cập nhật hàm handleSave
	const handleSave = () => {
		setValidationErrors({});

		if (!validateField(editField, editValue)) {
			return;
		}

		const fieldKey = editField.toLowerCase().replace(/\s+/g, '') as keyof ProfileData;

		setProfile(prev => ({
			...prev,
			[fieldKey]: editField === 'Languages' || editField === 'Work History'
				? editValue.split(',').map(item => item.trim())
				: editValue
		}));

		setIsEditPopupOpen(false);
		setValidationErrors({}); // Clear all validation errors
	};

	const [editableProfile, setEditableProfile] = useState<EditableProfile>({
		email: profile.email,
		languages: profile.languages.join(', '),
		nickname: profile.nickname,
		workHistory: profile.workHistory.join(', '),
		education: profile.education
	});

	const handleEditAll = () => {
		setEditableProfile({
			email: profile.email,
			languages: profile.languages.join(', '),
			nickname: profile.nickname,
			workHistory: profile.workHistory.join(', '),
			education: profile.education
		});
		setIsEditPopupOpen(true);
	};

	const handleSaveAll = () => {
		// Reset all validation errors
		setValidationErrors(prev => ({ ...prev }));

		// Validate all fields
		let hasErrors = false;

		for (const [field, value] of Object.entries(editableProfile)) {
			if (!validateField(field, value)) {
				hasErrors = true;
			}
		}

		// If there are any validation errors, stop here
		if (hasErrors) {
			return;
		}

		try {
			// Cập nhật profile nếu tất cả validation đều pass
			setProfile(prev => ({
				...prev,
				email: editableProfile.email,
				languages: editableProfile.languages.split(',').map(item => item.trim()),
				nickname: editableProfile.nickname,
				workHistory: editableProfile.workHistory.split(',').map(item => item.trim()),
				education: editableProfile.education
			}));

			setIsEditPopupOpen(false);
		} catch (error) {
			console.error('Error updating profile:', error);
			// Có thể thêm thông báo lỗi cho user ở đây
		}
	};

	useEffect(() => {
		fetchShotOwner()
	}, [])

	const fetchShotOwner = async () => {
		const data = await ApiGateway.ShotOwner<{
			id: string;
			image: string;
			title: string;
			countView: number;
			countLike: number;
			user: {
				username: string;
				image: string;
			};
			specialties: string[];
			datePosted: string;
		}[]>();
		if (Array.isArray(data)) {
			const formattedData: WorkPost[] = data.map((item) => ({
				id: item.id,
				title: item.title,
				image: item.image,
				views: item.countView,
				likes: item.countLike,
				category: item.specialties[0],
				datePosted: new Date(item.datePosted).toLocaleDateString("en-GB", {
					day: "2-digit",
					month: "2-digit",
					year: "numeric",
				})
			}));
			console.log(formattedData)
			setWorkPosts(formattedData);
		} else {
			console.error("Unexpected response format:", data);
		}
	}

	return (
		<div id="profile-page">
			<header className="profile-header">
				<div className="cover-image"></div>
				<div className="profile-info">
					<div className="avatar">
						<img src={avatarImg} alt="Profile Avatar" />
					</div>
					<div className="main-info">
						<div className="name-section">
							<h1>{profile.name}</h1>
							{/* <button className="like-btn">♡</button>
              <button className="edit-btn">Edit Profile</button> */}
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
								<div
									className="post-card work-card add-new-post"
									style={{ backgroundColor: '#E8E8E8', border: '0.1px solid #333333' }}
									onClick={hanldeAddPostClick}
								>
									<span className="plus-icon">+</span>
									<span>Add New Post</span>
								</div>
								{workPosts.map(post => (
									<div key={post.id} className="post-card work-card">
										<div className="post-image">
											<img src={post.image} alt={post.title} />
										</div>
										<div className="post-info">
											<span className="category">{post.category}</span>
											<h3>{post.title}</h3>
											<div className="post-meta">
												<span className="views">👁 {post.views}</span>
												<span className="likes">♥ {post.likes}</span>
												<span className="date">{post.datePosted}</span>
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
						<button
							className="edit-all-btn"
							onClick={handleEditAll}
							title="Edit Details"
						>
							<FiEdit2 />
						</button>
					</div>

					<div className="details-list">
						<div className="detail-item">
							<span className="label">Email :</span>
							<span className="value">{profile.email}</span>
						</div>

						<div className="detail-item">
							<span className="label">Languages :</span>
							<span className="value">{profile.languages.join(', ')}</span>
						</div>

						<div className="detail-item">
							<span className="label">Nickname :</span>
							<span className="value">{profile.nickname}</span>
						</div>

						<div className="detail-item">
							<span className="label">Work History</span>
							<span className="value">{profile.workHistory.join(', ')}</span>
						</div>

						<div className="detail-item">
							<span className="label">Education :</span>
							<span className="value">{profile.education}</span>
						</div>
					</div>

					{isEditPopupOpen && (
						<div className="edit-popup-overlay">
							<div className="edit-popup">
								<div className="edit-popup-header">
									<button
										className="close-btn"
										onClick={() => {
											setIsEditPopupOpen(false);
											setValidationErrors({});
										}}
									>
										×
									</button>
								</div>
								<div className="edit-popup-content">
									{Object.entries(editableProfile).map(([field, value]) => (
										<div key={field} className="edit-field">
											<label>{field.charAt(0).toUpperCase() + field.slice(1)}</label>
											<input
												type="text"
												value={value}
												onChange={(e) => {
													setEditableProfile(prev => ({
														...prev,
														[field]: e.target.value
													}));
													// Clear validation error for this field when typing
													setValidationErrors(prev => ({ ...prev, [field]: '' }));
												}}
												className={`edit-popup-input ${validationErrors[field] ? 'error' : ''
													}`}
												placeholder={`Enter ${field}`}
											/>
											{validationErrors[field] && (
												<div className="validation-message error">
													{validationErrors[field]}
												</div>
											)}
										</div>
									))}
								</div>
								<div className="edit-popup-actions">
									<button
										className="cancel-btn"
										onClick={() => {
											setIsEditPopupOpen(false);
											setValidationErrors({});
										}}
									>
										Cancel
									</button>
									<button
										className="save-btn"
										onClick={handleSaveAll}
									>
										Save All Changes
									</button>
								</div>
							</div>
						</div>
					)}
				</aside>
			</div>
		</div>
	)
}

export function DesProfile() {
	const [profile, setProfile] = useState<ProfileData>({
		name: "Rick Roll",
		title: "UI/UX Designer",
		location: "Ha Noi, Viet Nam",
		followers: 852,
		following: 156,
		email: "hunglcse161248@fpt.edu.vn",
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
			category: 'Mobile App',
			views: 1234,
			likes: 423,
			datePosted: '2024-03-15'
		},
		{
			id: '2',
			title: 'E-commerce Website Redesign',
			image: '/src/assets/Post Like.jpg',
			category: 'Web Design',
			views: 892,
			likes: 345,
			datePosted: '2024-03-10'
		},
		{
			id: '3',
			title: 'Food Delivery App UI Kit',
			image: '/src/assets/Post Like.jpg',
			category: 'UI Kit',
			views: 2156,
			likes: 567,
			datePosted: '2024-03-05'
		},
		{
			id: '4',
			title: 'Social Media Dashboard',
			image: '/src/assets/Post Like.jpg',
			category: 'Dashboard',
			views: 1567,
			likes: 289,
			datePosted: '2024-02-28'
		},
		{
			id: '5',
			title: 'Travel App Interface',
			image: '/src/assets/Post Like.jpg',
			category: 'Mobile App',
			views: 1890,
			likes: 456,
			datePosted: '2024-02-20'
		}
	]);

	const [activeTab, setActiveTab] = useState<string>('work');
	const [editValue, setEditValue] = useState('');
	const [editField, setEditField] = useState('');
	const [isEditPopupOpen, setIsEditPopupOpen] = useState(false);

	// Cập nhật state để theo dõi lỗi cho từng trường
	const [validationErrors, setValidationErrors] = useState<{ [key: string]: string }>({});

	// Định nghĩa rules cho từng trường
	const validationRules: ValidationRules = {
		email: {
			required: true,
			pattern: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
			errorMessage: 'Please enter a valid email address'
		},
		languages: {
			required: true,
			pattern: /^[a-zA-Z\s,]+$/,
			errorMessage: 'Languages should only contain letters, spaces, and commas'
		},
		nickname: {
			required: true,
			minLength: 3,
			maxLength: 20,
			pattern: /^[a-zA-Z0-9_]+$/,
			errorMessage: 'Nickname should be 3-20 characters and contain only letters, numbers, and underscores'
		},
		workHistory: {
			required: true,
			pattern: /^[a-zA-Z0-9\s,.-]+$/,
			errorMessage: 'Work history should not contain special characters'
		},
		education: {
			required: true,
			pattern: /^[a-zA-Z0-9\s,.-]+$/,
			errorMessage: 'Education should not contain special characters'
		}
	};

	// Cập nhật hàm validateField
	const validateField = (field: string, value: string): boolean => {
		const rules = validationRules[field.toLowerCase()];
		if (!rules) return true;

		// Clear error for this field
		setValidationErrors(prev => ({ ...prev, [field]: '' }));

		if (rules.required && !value.trim()) {
			setValidationErrors(prev => ({
				...prev,
				[field]: `${field} is required`
			}));
			return false;
		}

		if (rules.minLength && value.length < rules.minLength) {
			setValidationErrors(prev => ({
				...prev,
				[field]: `${field} must be at least ${rules.minLength} characters`
			}));
			return false;
		}

		if (rules.maxLength && value.length > rules.maxLength) {
			setValidationErrors(prev => ({
				...prev,
				[field]: `${field} must be less than ${rules.maxLength} characters`
			}));
			return false;
		}

		if (rules.pattern && !rules.pattern.test(value)) {
			setValidationErrors(prev => ({
				...prev,
				[field]: rules.errorMessage || `Invalid ${field} format`
			}));
			return false;
		}

		return true;
	};

	const handleTabClick = (tab: string) => (e: React.MouseEvent) => {
		e.preventDefault();
		setActiveTab(tab);
	};

	const handleChange = (field: keyof ProfileData, value: string) => {
		setProfile(prev => {
			const newProfile = { ...prev };

			// Handle arrays
			if (field === 'languages' || field === 'workHistory') {
				newProfile[field] = value.split(',').map(item => item.trim());
			} else {
				(newProfile as any)[field] = value;
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

	// Cập nhật hàm handleSave
	const handleSave = () => {
		setValidationErrors({});

		if (!validateField(editField, editValue)) {
			return;
		}

		const fieldKey = editField.toLowerCase().replace(/\s+/g, '') as keyof ProfileData;

		setProfile(prev => ({
			...prev,
			[fieldKey]: editField === 'Languages' || editField === 'Work History'
				? editValue.split(',').map(item => item.trim())
				: editValue
		}));

		setIsEditPopupOpen(false);
		setValidationErrors({}); // Clear all validation errors
	};

	const [editableProfile, setEditableProfile] = useState<EditableProfile>({
		email: profile.email,
		languages: profile.languages.join(', '),
		nickname: profile.nickname,
		workHistory: profile.workHistory.join(', '),
		education: profile.education
	});

	const handleEditAll = () => {
		setEditableProfile({
			email: profile.email,
			languages: profile.languages.join(', '),
			nickname: profile.nickname,
			workHistory: profile.workHistory.join(', '),
			education: profile.education
		});
		setIsEditPopupOpen(true);
	};

	const handleSaveAll = () => {
		// Reset all validation errors
		setValidationErrors(prev => ({ ...prev }));

		// Validate all fields
		let hasErrors = false;

		for (const [field, value] of Object.entries(editableProfile)) {
			if (!validateField(field, value)) {
				hasErrors = true;
			}
		}

		// If there are any validation errors, stop here
		if (hasErrors) {
			return;
		}

		try {
			// Cập nhật profile nếu tất cả validation đều pass
			setProfile(prev => ({
				...prev,
				email: editableProfile.email,
				languages: editableProfile.languages.split(',').map(item => item.trim()),
				nickname: editableProfile.nickname,
				workHistory: editableProfile.workHistory.split(',').map(item => item.trim()),
				education: editableProfile.education
			}));

			setIsEditPopupOpen(false);
		} catch (error) {
			console.error('Error updating profile:', error);
			// Có thể thêm thông báo lỗi cho user ở đây
		}
	};

	const quillRef = useRef<HTMLDivElement | null>(null);
	const quillInstance = useRef<Quill | null>(null);

	useEffect(() => {
		if (quillRef.current && !quillInstance.current) {
			quillInstance.current = new Quill(quillRef.current, {
				theme: 'snow',
				placeholder: 'Add your job description here',
				modules: {
					toolbar: [
						[{ header: '1' }, { header: '2' }, { font: [] }],
						['bold', 'italic', 'underline'],
						[{ list: 'ordered' }, { list: 'bullet' }]
					]
				}
			});
		}
	}, []);


	return (
		<div id="profile-page">
			<header className="profile-header">
				<div className="cover-image"></div>
				<div className="profile-info">
					<div className="avatar">
						<img src={avatarImg} alt="Profile Avatar" />
					</div>
					<div className="main-info">
						<div className="name-section">
							<h1>{profile.name}</h1>
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
						<button className="like-btn">
							<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24"><path fill="none" stroke="#606060" stroke-dasharray="32" stroke-dashoffset="32" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c0 0 0 0 -0.76 -1c-0.88 -1.16 -2.18 -2 -3.74 -2c-2.49 0 -4.5 2.01 -4.5 4.5c0 0.93 0.28 1.79 0.76 2.5c0.81 1.21 8.24 9 8.24 9M12 8c0 0 0 0 0.76 -1c0.88 -1.16 2.18 -2 3.74 -2c2.49 0 4.5 2.01 4.5 4.5c0 0.93 -0.28 1.79 -0.76 2.5c-0.81 1.21 -8.24 9 -8.24 9"><animate fill="freeze" attributeName="stroke-dashoffset" dur="0.7s" values="32;0" /></path></svg>
						</button>
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
							<span className="label">Email :</span>
							<span className="value">{profile.email}</span>
						</div>

						<div className="detail-item">
							<span className="label">Languages :</span>
							<span className="value">{profile.languages.join(', ')}</span>
						</div>

						<div className="detail-item">
							<span className="label">Nickname :</span>
							<span className="value">{profile.nickname}</span>
						</div>

						<div className="detail-item">
							<span className="label">Work History</span>
							<span className="value">{profile.workHistory.join(', ')}</span>
						</div>

						<div className="detail-item">
							<span className="label">Education :</span>
							<span className="value">{profile.education}</span>
						</div>
					</div>

					{isEditPopupOpen && (
						<div className="edit-popup-overlay">
							<div className="edit-popup">
								<div className="edit-popup-header">
									<button
										className="close-btn"
										onClick={() => {
											setIsEditPopupOpen(false);
											setValidationErrors({});
										}}
									>
										×
									</button>
								</div>
								<div className="edit-popup-content">
									{Object.entries(editableProfile).map(([field, value]) => (
										<div key={field} className="edit-field">
											<label>{field.charAt(0).toUpperCase() + field.slice(1)}</label>
											<input
												type="text"
												value={value}
												onChange={(e) => {
													setEditableProfile(prev => ({
														...prev,
														[field]: e.target.value
													}));
													// Clear validation error for this field when typing
													setValidationErrors(prev => ({ ...prev, [field]: '' }));
												}}
												className={`edit-popup-input ${validationErrors[field] ? 'error' : ''
													}`}
												placeholder={`Enter ${field}`}
											/>
											{validationErrors[field] && (
												<div className="validation-message error">
													{validationErrors[field]}
												</div>
											)}
										</div>
									))}
								</div>
								<div className="edit-popup-actions">
									<button
										className="cancel-btn"
										onClick={() => {
											setIsEditPopupOpen(false);
											setValidationErrors({});
										}}
									>
										Cancel
									</button>
									<button
										className="save-btn"
										onClick={handleSaveAll}
									>
										Save All Changes
									</button>
								</div>
							</div>
						</div>
					)}
				</aside>
			</div>
		</div>
	)
}
