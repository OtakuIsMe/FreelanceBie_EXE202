import React from 'react'
import "./Header.css"

const Header : React.FC = () => {
    return (
        <div id='main_header'>
            <div className='logo'>FreelanceBie</div>
            <div className='navbar'>
                <ul>
                    <li><a href="/inspriration">Explore</a></li>
                    <li><a href="/search-designer">Hire a Designer</a></li>
                    <li><a href="#">Find Jobs</a></li>
                </ul>
            </div>
            <div className='search'>
                <p>What are you looking for</p>
            </div>
            <div className='auth'>
                <button className='login btn'>Sign in</button>
                <button className='register btn'>Sign up</button>
            </div>
        </div>
    )
}

export default Header