import { useState, useEffect, useRef } from 'react'
import './Profile.css'
import { MdEmail } from "react-icons/md";
import { FaFacebook, FaTwitter, FaInstagram, FaBriefcase, FaClipboard, FaHourglass, FaDollarSign } from "react-icons/fa";
import avatarImg from '../../../assets/avatar.jpg'
import { FiEdit2 } from 'react-icons/fi';
import { useSearchParams } from "react-router-dom";
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

	const [imagePreview, setImagePreview] = useState<string | null>(null);

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

	const [isPopupOpen, setIsPopupOpen] = useState(false);
	const [imagePreview, setImagePreview] = useState<string | null>(null);
	const quillRef = useRef<HTMLDivElement | null>(null);
	const quillInstance = useRef<Quill | null>(null);

	const handleAddNewPost = () => {
		setIsPopupOpen(true);
	};

	const handleClosePopup = () => {
		setIsPopupOpen(false);
		setImagePreview(null); // Reset image preview on close
	};

	const handleImageChange = (event: React.ChangeEvent<HTMLInputElement>) => {
		const file = event.target.files?.[0];
		if (file) {
			const reader = new FileReader();
			reader.onloadend = () => {
				setImagePreview(reader.result as string);
			};
			reader.readAsDataURL(file);
		}
	};

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

	const [currentStep, setCurrentStep] = useState(1);

	const handleNextStep = () => {
		setCurrentStep(prevStep => Math.min(prevStep + 1, 3));
	};

	const handlePreviousStep = () => {
		setCurrentStep(prevStep => Math.max(prevStep - 1, 1));
	};

	// Define jobTitle, jobDescription, companyName, and companyLogo in the component's state
	const [jobTitle] = useState('');
	const [jobDescription] = useState('');
	const [companyName] = useState('');
	const [companyLogo] = useState('');

	// Update the state values when the user fills in the information in Step 1 and Step 2
	// For example, in Step 1 form inputs, you can update the state like this:
	// const handleJobTitleChange = (e) => {
	//   setJobTitle(e.target.value);
	// }

	// Pass these state values as props to the component or update them accordingly

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

			{isPopupOpen && (
				<div className="popup-overlay-add-post">
					<div className="popup-content">
						<button className="close-btn-add-post" onClick={handleClosePopup}>×</button>
						<h2 className="popup-title">Post a Design Job</h2>
						<p className="popup-description">Create job board for hiring designers</p>
						<div className="step-indicator">
							<div className={`step ${currentStep === 1 ? 'active' : ''}`}>
								<span className="step-number">1</span>
								<span className="step-label">Job Details</span>
							</div>
							<div className={`step ${currentStep === 2 ? 'active' : ''}`}>
								<span className="step-number">2</span>
								<span className="step-label">Requirement & Date</span>
							</div>
							<div className={`step ${currentStep === 3 ? 'active' : ''}`}>
								<span className="step-number">3</span>
								<span className="step-label">Confirm & Complete</span>
							</div>
						</div>
						<div className="form-step">
							{currentStep === 1 && (
								<>
									<h3>Step 1</h3>
									<h4>Job Details</h4>
									<form>
										<div className="form-group">
											<label htmlFor="title">Job Title</label>
											<input type="text" id="title" name="title" placeholder="e.g. Senior Product Designer" />
										</div>
										<div className="form-group">
											<label>Add your job description</label>
											<div className="rich-text-editor">
												<div ref={quillRef} />
											</div>
										</div>
										<div className="form-group">
											<label htmlFor="workplace">Workplace Type</label>
											<input type="text" id="workplace" name="workplace" placeholder='e.g. "New York City" or' />
										</div>
										<div className="company-information">
											<h4>Company Information</h4>
											<div className="form-group">
												<label htmlFor="companyName">What's your company name?</label>
												<input type="text" id="companyName" name="companyName" placeholder='e.g. "FreelanceBie" or ...' />
											</div>
											<div className="form-group">
												<label htmlFor="companyLogo">Your company logo</label>
												<div className="file-input-wrapper">
													<label htmlFor="companyLogo" className="file-label">Choose image</label>
													<input type="file" id="companyLogo" name="companyLogo" onChange={handleImageChange} />
												</div>
												<small>Recommended dimensions: 144x144px</small>
												{imagePreview && <img src={imagePreview} alt="Company Logo Preview" className="image-preview" />}
											</div>
											<div className="form-group">
												<label htmlFor="companyWebsite">Your company website</label>
												<input type="text" id="companyWebsite" name="companyWebsite" placeholder="e.g. https://domain.com" />
											</div>
										</div>

									</form>
								</>
							)}
							{currentStep === 2 && (
								<div className="step-2">
									<h3>Step 2</h3>
									<h4>Requirement & Date</h4>
									<form>
										<div className="form-group">
											<label>Employment type</label>
											<div className="employment-type">
												<label className="employment-option">
													<input type="radio" name="employmentType" value="fullTime" />
													<span className="icon"><i className="fas fa-briefcase"></i></span> Full Time
												</label>
												<label className="employment-option">
													<input type="radio" name="employmentType" value="partTime" />
													<span className="icon"><i className="fas fa-clipboard"></i></span> Part Time
												</label>
												<label className="employment-option">
													<input type="radio" name="employmentType" value="contract" />
													<span className="icon"><i className="fas fa-hourglass"></i></span> Contract
												</label>
											</div>
										</div>

										<div className="form-group">
											<label>What type of design are you looking for?</label>
											<input type="text" placeholder="e.g. UI/UX Design" />
										</div>

										<div className="form-group">
											<label>Salary type</label>
											<div className="salary-type">
												<label className="salary-option">
													<input type="radio" name="salaryType" value="hourly" />
													<span>Hourly</span>
												</label>
												<label className="salary-option">
													<input type="radio" name="salaryType" value="monthly" />
													<span>Monthly</span>
												</label>
											</div>
										</div>

										<div className="form-group">
											<label>Payment rate</label>
											<div className="payment-rate">
												<span>
													<FaDollarSign />
												</span>
												<input type="number" placeholder="10" />
												<span>/hour</span>
											</div>
										</div>
									</form>
								</div>
							)}
							{currentStep === 3 && (
								<>
									<h3>Step 3</h3>
									<h4>Confirm & Complete</h4>
									{/* Display job title and job description from Step 1 */}
									<div>
										<h5>Job Title:</h5>
										<p>{jobTitle}</p>
										<h5>Job Description:</h5>
										<p>{jobDescription}</p>
									</div>
									{/* Display company name and company logo from Step 2 */}
									<div>
										<h5>Company Name:</h5>
										<p>{companyName}</p>
										<h5>Company Logo:</h5>
										<img src={companyLogo} alt="Company Logo" />
									</div>
								</>
							)}
						</div>
						<div className="form-actions">
							<button type="button" className="cancel-btn" onClick={handleClosePopup}>Cancel</button>
							{currentStep > 1 && (
								<button type="button" className="back-btn" onClick={handlePreviousStep}>Back</button>
							)}
							{currentStep < 3 ? (
								<button type="button" className="continue-btn" onClick={handleNextStep}>Continue</button>
							) : (
								<button type="submit" className="submit-btn">Submit</button>
							)}
						</div>
					</div>
				</div>
			)}
		</div>
	)
}
