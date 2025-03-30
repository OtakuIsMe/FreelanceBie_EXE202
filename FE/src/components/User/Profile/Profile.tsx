import { useState, useEffect } from 'react'
import './Profile.css'
import { MdEmail } from "react-icons/md";
import { FaFacebook, FaTwitter, FaInstagram } from "react-icons/fa";
import { FiEdit2 } from 'react-icons/fi';
import { ApiGateway } from '../../../services/api/ApiService';
import 'quill/dist/quill.snow.css';

interface ProfileData {
	name: string;
	slogan: string;
	location: string;
	email: string;
	languages: string;
	username: string;
	joinDate: string;
	education: string;
	image: string
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
	education: string;
}

export default function Profile() {

	const [profile, setProfile] = useState<ProfileData>({
		name: "",
		slogan: "",
		location: "",
		email: "",
		languages: "",
		username: "",
		joinDate: "",
		education: "",
		image: ""
	});

	const [likedPosts, setLikedPosts] = useState<LikedPost[]>([]);

	const [workPosts, setWorkPosts] = useState<WorkPost[]>([]);

	const [activeTab, setActiveTab] = useState<string>('work');
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

	useEffect(() => {
		fetchingUser();
	}, [])

	const fetchingUser = async () => {
		const data = await ApiGateway.Profile<ProfileData>()
		setProfile(data);
	}

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

	const [editableProfile, setEditableProfile] = useState<EditableProfile>({
		email: profile.email,
		languages: profile.languages,
		nickname: profile.username,
		education: profile.education
	});

	const handleEditAll = () => {
		setEditableProfile({
			email: profile.email,
			languages: profile.languages,
			nickname: profile.username,
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
				languages: editableProfile.languages,
				nickname: editableProfile.nickname,
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
		fetchLikedShot()
	}, [])

	const fetchLikedShot = async () => {
		const data = await ApiGateway.LikedShot<LikedPost[]>()
		setLikedPosts(data);
	}

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
						<img src={profile.image} alt="Profile Avatar" />
					</div>
					<div className="main-info">
						<div className="name-section">
							<h1>{profile.name ? profile.name : profile.username ? profile.username : "User"}</h1>
						</div>
						<p className="location">{profile.location}</p>
						<p className="title">{profile.slogan}</p>
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
							className={activeTab === 'liked' ? 'active' : ''}
							onClick={handleTabClick('liked')}
						>
							Liked Posts
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
							<span className="value">{profile.email ? profile.email : "user@gmail.com"}</span>
						</div>

						<div className="detail-item">
							<span className="label">Languages :</span>
							<span className="value">{profile.languages ? profile.languages : "Vietnamese"}</span>
						</div>

						<div className="detail-item">
							<span className="label">Nickname :</span>
							<span className="value">{profile.username ? profile.username : " User"}</span>
						</div>

						{profile.education && (
							<div className="detail-item">
								<span className="label">Education :</span>
								<span className="value">{profile.education}</span>
							</div>
						)}
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
