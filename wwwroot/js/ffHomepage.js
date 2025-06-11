// Sidebar functionality constants
const burgerMenu = document.getElementById('burgerMenu');
const burgerIcon = document.getElementById('burgerIcon');
const sidebar = document.getElementById('sidebar');
const closeSidebar = document.getElementById('closeSidebar');
const sidebarOverlay = document.getElementById('sidebarOverlay');

//to open the sidebar
function openSidebar() {
    sidebar.classList.add('open');
    sidebarOverlay.classList.add('active');
    burgerIcon.classList.add('open');
    document.body.style.overflow = 'hidden';
}

//to close the sidebar
function closeSidebarFunc() {
    sidebar.classList.remove('open');
    sidebarOverlay.classList.remove('active');
    burgerIcon.classList.remove('open');
    document.body.style.overflow = 'auto';  //to allow scrolling again
}

if (burgerMenu) burgerMenu.addEventListener('click', openSidebar);  //it calls the openSidebar when the burger menu is clicked
if (closeSidebar) closeSidebar.addEventListener('click', closeSidebarFunc); // calls the closesidebarFunc when the cancel button is clicked
if (sidebarOverlay) sidebarOverlay.addEventListener('click', closeSidebarFunc);

// Close sidebar when clicking on menu items
const sidebarLinks = document.querySelectorAll('.sidebar-menu a'); //selects the elements within the sidebar menus then store it in sidebarlinks
sidebarLinks.forEach(link => {  // iterate over each links in sidebarlinks
    link.addEventListener('click', (e) => {  //if the link is clicked, the sidebar will close
        closeSidebarFunc();
    });
});

// Close sidebar with Escape key
document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape' && sidebar && sidebar.classList.contains('open')) {
        closeSidebarFunc();
    }
});
document.addEventListener('DOMContentLoaded', () => {
    // Carousel functionality - Keep all existing animations
    let next = document.getElementById('next');
    let prev = document.getElementById('prev');
    let carousel = document.querySelector('.carousel');
    let items = document.querySelectorAll('.carousel .item');
    let dots = document.querySelectorAll('.pagination-dots .dot');

    let countItem = items.length; // stores the number of items in the carousel
    let active = 1; // Start with middle slide active
    let other_1 = null;  //null for now, to be used to tracked other items
    let other_2 = null;

    // Function to update pagination dots
    const updateDots = () => {
        console.log('Updating dots, active slide:', active);
        dots.forEach((dot, index) => {
            dot.classList.remove('active'); // Remove active from all dots first
        });
        
        // Add active class to the correct dot based on active slide
        if (dots[active]) {
            dots[active].classList.add('active');
        }
    }
    
    //for next arrow
    next.onclick = () => { //updates the prev and next slides
        carousel.classList.remove('prev'); //remove kase mapapalitan na nang bagong 'prev' kapag umikot na ulit yung slide
        carousel.classList.add('next');
        active = active + 1 >= countItem ? 0 : active + 1;
        other_1 = active - 1 < 0 ? countItem - 1 : active - 1;
        other_2 = active + 1 >= countItem ? 0 : active + 1;
        changeSlider(); //fuction to update carousel 
    }

    //for prev arrow
    prev.onclick = () => {
        carousel.classList.remove('next');
        carousel.classList.add('prev');
        active = active - 1 < 0 ? countItem - 1 : active - 1;
        other_1 = active + 1 >= countItem ? 0 : active + 1;
        other_2 = other_1 + 1 >= countItem ? 0 : other_1 + 1;
        changeSlider();
    }

     //to update the carousel
    const changeSlider = () => {
        let itemOldActive = document.querySelector('.carousel .item.active');  //selects the current active and removes the active class from it
        if(itemOldActive) itemOldActive.classList.remove('active');

        let itemOldOther_1 = document.querySelector('.carousel .item.other_1');
        if(itemOldOther_1) itemOldOther_1.classList.remove('other_1');

        let itemOldOther_2 = document.querySelector('.carousel .item.other_2');
        if(itemOldOther_2) itemOldOther_2.classList.remove('other_2');

        items.forEach(e => {  //iterates each item in the carousel
            if (e.querySelector('.image img')) {
                e.querySelector('.image img').style.animation = 'none';
                void e.offsetWidth;
                e.querySelector('.image img').style.animation = '';
            }
        })

        items[active].classList.add('active');
        items[other_1].classList.add('other_1');
        items[other_2].classList.add('other_2');

        // Update pagination dots
        updateDots();

        clearInterval(autoPlay);  //autoplay of animation
        autoPlay = setInterval(() => {
            next.click();
        }, 5000);  //auto-next in 5 seconds
    }

    // Add dot click functionality - Fixed indexing
    dots.forEach((dot, index) => {
        dot.addEventListener('click', () => {
            console.log('Dot clicked:', index, 'Current active:', active);
            
            if (index === active) return; // Don't do anything if clicking current slide
            
            // Clear any existing animations
            carousel.classList.remove('next', 'prev');
            
            // Determine direction for animation
            let direction;
            if (index > active) {
                direction = 'next';
            } else {
                direction = 'prev';
            }
            
            // Set the new active slide
            active = index;
            
            // Dynamic calculation for any number of items
            other_1 = (active - 1 + countItem) % countItem;
            other_2 = (active + 1) % countItem;

            
            // Apply animation class
            carousel.classList.add(direction);
            
            // Change the slider
            setTimeout(() => {
                changeSlider();
                carousel.classList.remove(direction);
            }, 100);
        });
    });

    let autoPlay = setInterval(() => {
        next.click();
    }, 5000);

    // Initialize dots on page load
    updateDots();
});